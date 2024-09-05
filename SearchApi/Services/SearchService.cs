using SearchApi.Data;
using SearchApi.Models.Entities;
using System;
using System.Net;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Microsoft.Extensions.Caching.Memory;
using System.Buffers;
using static System.Net.WebRequestMethods;
using SearchApi.Models.DTO;


namespace SearchApi.Services
{
    public class SearchService : ISearchService
    {
        private readonly ILogger<SearchService> _logger;
        private readonly AppDBContext _appDBContext;
        private readonly ICacheService _cacheService;
        public SearchService(ILogger<SearchService> logger, AppDBContext appDBContext, ICacheService cacheService)// IMemoryCache cache)
        {
             _logger = logger;
            _appDBContext = appDBContext;   
            _cacheService = cacheService;
        }
        /// <summary>
        /// Get search results from google and bing of the requested value 
        /// </summary>
        /// <param name="SearchVal">The requested value for search</param>
        /// <returns> The search results of the requested value</returns>
        /// <remarks>
        /// First, the function will check in the cache whether a search has already been made for the 
        /// requested value. If so - it will be returned. If not - Google and Bing search functions
        /// will be activated at the same time and then the search results will be save in cache & DB.
        /// </remarks>
        public async Task<IEnumerable<SearchResultDTO>> SearchAsync(string SearchVal)
        {
            try
            {
                var searchResults = _cacheService.GetItem<IEnumerable<SearchResultDTO>>(SearchVal);
                if (searchResults is null)
                {
                    var taskBingSearch = BingSearch(SearchVal);
                    var taskGoogleSearch = GoogleSearch(SearchVal);

                    await Task.WhenAll(taskBingSearch, taskGoogleSearch);
                    searchResults = taskBingSearch.Result.Concat(taskGoogleSearch.Result);
                    _cacheService.SaveItem<IEnumerable<SearchResultDTO>>(SearchVal, searchResults);
                    SaveResultsInDB(searchResults);
                }
                return searchResults;
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred in SearchService.cs at SearchAsync function: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Search the requested value in google
        /// </summary>
        /// <param name="SearchVal">The requested value for search</param>
        /// <returns> The search results of the requested value in Google </returns>
        /// <remarks>
        /// This method is used for  a Google search of the requested value.
        /// After receiving the results, a conversion to an object of type IEnumerable<SearchResultDTO>
        /// is performed
        /// </remarks>
        private async Task<IEnumerable<SearchResultDTO>> GoogleSearch(string SearchVal)
        {
            try
            {
                List<SearchResultDTO> resDto = new List<SearchResultDTO>(){};
                var searchMode = new ChromeOptions();
                searchMode.AddArgument("headless");

                using (WebDriver webDriver = new ChromeDriver(searchMode))
                {
                    webDriver.Url = "https://www.google.com/search?q=" + SearchVal + "&num=5";
                    var pages = webDriver.FindElements(By.ClassName("LC20lb"));
                    string res = string.Empty;
                    foreach (var page in pages)
                    {
                        if (!string.IsNullOrWhiteSpace(page.Text))
                        {
                            SearchResultDTO dto = new SearchResultDTO(){EnteredDate = DateTime.Now,SearchEngine = "google",Title = page.Text,Url = "TDB"};
                            resDto.Add(dto);
                        }
                    }
                }
                return resDto;
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred in SearchService.cs at GoogleSearch function:: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Search the requested value in bing
        /// </summary>
        /// <param name="SearchVal">The requested value for search</param>
        /// <returns> The search results of the requested value in Bing </returns>
        /// <remarks>
        /// This method is used for a Bing search of the requested value.
        /// After receiving the results, a conversion to an object of type IEnumerable<SearchResultDTO>
        /// is performed
        /// Unfortunately, the function doesn't work, so it returns dummy data
        /// </remarks>
        private async Task<IEnumerable<SearchResultDTO>> BingSearch(string SearchVal)
        {
            try
            {
                SearchResultDTO dto1 = new SearchResultDTO() { EnteredDate = DateTime.Now, SearchEngine = "bing", Title = SearchVal, Url = "link1" };
                SearchResultDTO dto2 = new SearchResultDTO() { EnteredDate = DateTime.Now, SearchEngine = "bing", Title = SearchVal, Url = "link2" };
                IEnumerable<SearchResultDTO> resDto = new List<SearchResultDTO>() { dto1, dto2 };
                return resDto;

                /*
                var searchMode = new ChromeOptions();
                searchMode.AddArgument("headless");

                WebDriver webDriver = new ChromeDriver(searchMode);
                webDriver.Url = $"https://www.bing.com/search?q={SearchVal}&count=5";
                //var pages = webDriver.FindElements(By.XPath("//li[@class='b_algo']//h2/a"));
                var pages = webDriver.FindElements(By.XPath("//*[contains(@class,'b_algo')]//h2/a"));
           
                List<SearchResultDTO> resDto1 = new List<SearchResultDTO>(){};
                foreach (var page in pages)
                {
                    if (!string.IsNullOrWhiteSpace(page.Text))
                    {
                        SearchResultDTO dto = new SearchResultDTO(){EnteredDate = DateTime.Now, SearchEngine = "bing", Title = page.Text, Url = "TDB" };
                        resDto1.Add(dto);
                    }
                }
                */
                
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred in SearchService.cs at BingSearch function: {ex.Message}");
                throw;
            }
        }
        /// <summary>
        /// Save the search results in DB 
        /// </summary>
        /// <param name="searchResultsDTO"> The search results as IEnumerable<SearchResultDTO></param>
        /// <returns>Nothing</returns>
        /// <remarks>
        /// This method is used for save the search results in a database.
        /// Before saving it will be converted to the appropriate object for saving: SearchResult
        /// </remarks>
        public void SaveResultsInDB(IEnumerable<SearchResultDTO> searchResultsDTO) 
        {
            try
            {
               var searchResultsEntites = searchResultsDTO.AsParallel().Select(dto => new SearchResult
               {
                   Title = dto.Title,
                   SearchEngine = dto.SearchEngine,
                   Url = dto.Url,
                   EnteredDate = dto.EnteredDate
               });

               _appDBContext.SearchResults.AddRange(searchResultsEntites);
               _appDBContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogError($"an error occurred in SearchService.cs at SaveResults function: {ex.Message}");
                throw;
            }
        }
    }
}
