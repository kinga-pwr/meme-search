using Nest;

namespace MemeSearch.Logic.Models
{
    [ElasticsearchType(RelationName = "meme")]
    public class Meme
    {
        [Keyword]
        public string Title { get; set; }

        [Text]
        public string Content { get; set; }
        
        [Text]
        public string ImageUrl { get; set; }

        [Text]
        public string ImageTags { get; set; }

        [Keyword]
        public string Status { get; set; }

        [Keyword]
        public string Details { get; set; }

        [Date(Format = "yyyy")]
        public int Year { get; set; }

        [Keyword]
        public string Category { get; set; }
        
        [Text]
        public string Url { get; set; }

        [Text(Ignore = true)]
        public string ContentHighlight { get; set; }
    }
}
