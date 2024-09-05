using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SearchApi.Models;
using SearchApi.Services;

namespace SearchApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SearchController : ControllerBase
    {
        private readonly ISearchService _searchService;
        private readonly ILogger<SearchController> _logger;

        public SearchController(ISearchService searchService, ILogger<SearchController> logger)
        {
            _searchService = searchService;
            _logger = logger;
        }

        [HttpGet]
        public  async Task<ActionResult> Get(string searchVal)
        {
            try
            {
                if (string.IsNullOrEmpty(searchVal))
                {
                    return BadRequest();
                }
                var searchResults = await _searchService.SearchAsync(searchVal);
                return Ok(searchResults); 
            }
            catch (Exception ex)
            { 
                _logger.LogError($"an error occurred at SearchController: {ex.Message}");
                return  StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }

    /// <summary>
    /// Adds two integers and returns the result.
    /// </summary>
    /// <param name="a">The first integer to add.</param>
    /// <param name="b">The second integer to add.</param>
    /// <returns>The sum of the two integers.</returns>
    /// <remarks>
    /// This method is used for simple addition of integers and assumes 
    /// that no overflow will occur. Consider using checked arithmetic if 
    /// you expect the sum to exceed the limits of an integer.
    /// </remarks>
}
