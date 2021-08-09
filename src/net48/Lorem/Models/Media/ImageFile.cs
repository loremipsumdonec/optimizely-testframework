using EPiServer.DataAbstraction;
using EPiServer.DataAnnotations;
using EPiServer.Framework.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Media
{
    [ContentType(GUID = "183A753E-ACFB-4EE9-8EF1-423D1953D238")]
    [MediaDescriptor(ExtensionString = "jpg,jpeg,gif,bmp,png,webp,svg")]
    public class ImageFile
        : SiteImageFile
    {
        [Display(GroupName = SystemTabNames.Content, Order = 15)]
        public virtual string Alt { get; set; }

        [Display(GroupName = SystemTabNames.Content, Order = 20)]
        public virtual string Description { get; set; }
    }
}