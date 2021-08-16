using EPiServer.Core;

namespace Lorem.Test.Framework.Optimizely.CMS.Utility
{
    public static class BlockDataExtensions
    {
        public static ContentReference GetContentLink(this BlockData block)
        {
            return ((IContent)block).ContentLink;
        }

        public static ContentReference GetParentLink(this BlockData block)
        {
            return ((IContent)block).ParentLink;
        }
    }
}
