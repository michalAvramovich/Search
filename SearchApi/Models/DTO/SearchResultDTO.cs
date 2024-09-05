namespace SearchApi.Models.DTO
{
    public class SearchResultDTO
    {
        public required string SearchEngine { get; set; }
        public required string Title { get; set; }
        public required DateTime EnteredDate { get; set; }
        public required string Url { get; set; }
    }
}
