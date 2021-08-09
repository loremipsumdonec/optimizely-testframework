using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Media
{

    public abstract class SiteImageFile
        : ImageData
    {
        [Display(GroupName = SystemTabNames.Content, Order = 10)]
        public virtual string Heading { get; set; }
    }
}