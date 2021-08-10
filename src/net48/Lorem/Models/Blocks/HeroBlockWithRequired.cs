using EPiServer.DataAnnotations;
using System.ComponentModel.DataAnnotations;

namespace Lorem.Models.Blocks
{
    [ContentType(DisplayName = "Hero block with required", GUID = "2265D62E-EB6B-4D23-B6D7-5CEECBD5C830")]
    public class HeroBlockWithRequired
        : SiteBlock
    {
        [Required]
        public override string Heading { get; set; }
    }
}
