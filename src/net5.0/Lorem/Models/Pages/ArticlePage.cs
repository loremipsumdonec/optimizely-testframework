using EPiServer.DataAnnotations;

namespace Lorem.Models.Pages
{
    [ContentType(DisplayName = "Article page", GUID = "52ED87C4-231B-4138-9C5C-1BF9DF3B1F52")]
    public class ArticlePage
        : SitePage
    {
        public virtual string Preamble { get; set; }
    }
}
