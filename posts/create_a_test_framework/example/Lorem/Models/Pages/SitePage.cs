using EPiServer.Core;
using EPiServer.DataAbstraction;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Pages
{
    public abstract class SitePage
        : PageData
    {
        [Display(GroupName = SystemTabNames.PageHeader, Order = 100)]
        public virtual bool VisibleInBreadCrumb { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 10)]
        public virtual string Heading { get; set; }

        [Display(GroupName = "Puff", Order = 10)]
        public virtual string PuffHeading { get; set; }
    }
}