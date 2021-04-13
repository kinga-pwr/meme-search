namespace MemeSearch.Logic.Models
{
    public class SearchParameters
    {
        public string Status { get; set; }

        public int? YearFrom { get; set; }

        public int? YearTo { get; set; }

        public string[] Fields { get; set; } = { "Title", "Content", "Category", "Details" };
    }
}
