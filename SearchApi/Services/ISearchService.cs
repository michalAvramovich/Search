using SearchApi.Models.DTO;

namespace SearchApi.Services
{
    public interface ISearchService
    {
        Task<IEnumerable<SearchResultDTO>> SearchAsync(string SearchVal);
    }
}