using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Web;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Pages
{
    [ContentType(DisplayName = "Article page", GUID = "198AEC77-9CB9-42F0-BC9F-9A0888A3F5AF")]
    public class ArticlePage
        : SitePage
    {
        [UIHint(UIHint.Textarea)]
        [Display(GroupName = SystemTabNames.Content, Order = 2)]
        public virtual string Preamble { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 3)]
        public virtual XhtmlString Text { get; set; }
    }
}