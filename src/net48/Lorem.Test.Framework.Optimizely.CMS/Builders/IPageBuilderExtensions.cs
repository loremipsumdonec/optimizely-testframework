using EPiServer.Core;
using System;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public static class IPageBuilderExtensions
    {
        internal static ISiteBuilder<T> CreateSite<T>(this IPageBuilder<T> _, Fixture fixture)
            where T : PageData => new SiteBuilder<T>(fixture).CreateSite();

        internal static ISiteBuilder<T> CreateSite<T>(this IPageBuilder<T> _, Fixture fixture, string name, string url)
            where T : PageData => new SiteBuilder<T>(fixture).CreateSite(name, url);
    }
}
