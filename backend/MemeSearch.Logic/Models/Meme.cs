using Nest;

namespace MemeSearch.Logic.Models
{
    [ElasticsearchType(RelationName = "meme")]
    public class Meme
    {
        [Keyword(Normalizer= "keywords")]
        public string Title { get; set; }

        [Text]
        public string Content { get; set; }
        
        [Text]
        public string ImageUrl { get; set; }

        [Text]
        public string ImageTags { get; set; }

        [Text]
        public string ImageTagsFull { get; set; }

        [Keyword(Normalizer = "keywords")]
        public string Status { get; set; }

        [Text]
        public string Details { get; set; }

        [Number]
        public int Year { get; set; }

        [Text]
        public string Category { get; set; }
        
        [Text]
        public string Url { get; set; }

        [Text(Ignore = true)]
        public string ContentHighlight { get; set; }
    }
}
