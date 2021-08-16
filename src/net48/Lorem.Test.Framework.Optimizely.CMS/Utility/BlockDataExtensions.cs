using EPiServer.Core;
using System;

namespace Lorem.Test.Framework.Optimizely.CMS.Utility
{
    public static class BlockDataExtensions
    {
        public static Guid GetContentGuid(this BlockData block)
        {
            return ((IContent)block).ContentGuid;
        }

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
