using EPiServer.DataAnnotations;

namespace Lorem.Models.Blocks
{
    [ContentType(DisplayName = "Hero block", GUID = "31191D1A-CD6C-4DCC-837F-EB745A1A4758")]
    public class HeroBlock
        : SiteBlock
    {
        public virtual string Heading { get; set; }
    }
}
