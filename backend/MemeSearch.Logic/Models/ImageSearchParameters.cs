namespace MemeSearch.Logic.Models
{
    public class ImageSearchParameters : SearchParameters
    {
        public string Url { get; set; }
        public bool SearchSimilarities { get; set; }
    }
}
