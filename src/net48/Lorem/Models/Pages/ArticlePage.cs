using EPiServer.Core;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Pages
{
    [ContentType(DisplayName = "Article page", GUID = "52ED87C4-231B-4138-9C5C-1BF9DF3B1F52")]
    public class ArticlePage
        : SitePage
    {
        [UIHint(UIHint.Image)]
        [CultureSpecific]
        public virtual ContentReference TopImage { get; set; }

        public virtual string Preamble { get; set; }
    }
}
