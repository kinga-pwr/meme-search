using System.Collections.Generic;

namespace MemeSearch.Logic.Models
{
    public class TextSearchResult
    {
        public long NumberOfResults { get; set; }
        public IEnumerable<MemeDto> Memes { get; set; }
    }
}
