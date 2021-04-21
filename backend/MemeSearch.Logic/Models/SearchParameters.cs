using System.Collections.Generic;

namespace MemeSearch.Logic.Models
{
    public class SearchParameters
    {
        public IEnumerable<string> Status { get; set; } = new List<string>();

        public IEnumerable<string> Category { get; set; } = new List<string>();

        public IEnumerable<string> Details { get; set; } = new List<string>();

        public int? YearFrom { get; set; }

        public int? YearTo { get; set; }

        public IEnumerable<string> Fields { get; set; } = new List<string> { "Title", "Content", "Category", "Details", "Image" };

        public string Sort { get; set; }

        public bool SortAsc { get; set; }
    }
}
