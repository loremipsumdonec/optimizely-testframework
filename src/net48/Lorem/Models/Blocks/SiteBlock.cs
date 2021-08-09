using EPiServer.Core;

namespace Lorem.Models.Blocks
{
    public abstract class SiteBlock
        : BlockData
    {
        public virtual string Heading { get; set; }
    }
}
