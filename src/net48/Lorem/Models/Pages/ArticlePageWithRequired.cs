using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Pages
{
    [ContentType(DisplayName = "Article page with required", GUID = "44535A0D-E558-4EF2-9B5A-E10A95A9B8F0")]
    public class ArticlePageWithRequired
        : SitePage
    {
        [Required]
        public virtual string Preamble { get; set; }
    }
}
