using System.Collections.Generic;

namespace MemeSearch.Logic.Models
{
    public class ImageSearchResult
    {
        public string Tags { get; set; }
        public IEnumerable<MemeDto> Memes { get; set; }
    }
}
