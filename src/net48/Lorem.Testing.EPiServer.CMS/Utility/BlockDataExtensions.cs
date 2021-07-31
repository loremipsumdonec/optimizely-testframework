using EPiServer.Core;

namespace Lorem.Testing.EPiServer.CMS.Utility
{
    public static class BlockDataExtensions
    {
        public static ContentReference GetContentLink(this BlockData block)
        {
            return ((IContent)block).ContentLink;
        }
    }
}
