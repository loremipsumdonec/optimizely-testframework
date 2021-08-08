using EPiServer.Core;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public static class IPageBuilderExtensions
    {
        public static ISiteBuilder<T> CreateSite<T>(this IPageBuilder<T> _, EpiserverFixture fixture)
            where T : PageData => new SiteBuilder<T>(fixture).CreateSite();

        public static ISiteBuilder<T> CreateSite<T>(this IPageBuilder<T> _, EpiserverFixture fixture, string name, string url)
            where T : PageData => new SiteBuilder<T>(fixture).CreateSite(name, url);
    }
}
