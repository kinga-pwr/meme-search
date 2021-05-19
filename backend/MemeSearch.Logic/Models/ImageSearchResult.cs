namespace MemeSearch.Logic.Models
{
    public class ImageSearchResult : TextSearchResult
    {
        public string Tags { get; set; }

        public ImageSearchResult() { }

        public ImageSearchResult(TextSearchResult result) 
        {
            NumberOfResults = result.NumberOfResults;
            Memes = result.Memes;
        }
    }
}
