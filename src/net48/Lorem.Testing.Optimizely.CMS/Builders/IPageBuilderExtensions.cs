using EPiServer.Core;
using System;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public static class IPageBuilderExtensions
    {
        internal static ISiteBuilder<T> CreateSite<T>(this IPageBuilder<T> _, EpiserverFixture fixture)
            where T : PageData => new SiteBuilder<T>(fixture).CreateSite();

        internal static ISiteBuilder<T> CreateSite<T>(this IPageBuilder<T> _, EpiserverFixture fixture, string name, string url)
            where T : PageData => new SiteBuilder<T>(fixture).CreateSite(name, url);
    }
}
