using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Pages
{
    [ContentType(DisplayName = "Start page", GUID = "AF8153A7-4F25-4255-B91D-43A7F04A29D2")]
    public class StartPage
        : SitePage
    {
        [Display(GroupName = SystemTabNames.Content, Order = 1)]
        public virtual string Heading { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 2)]
        public virtual ContentArea ContentArea { get; set; }
    }
}