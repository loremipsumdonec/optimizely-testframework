using EPiServer.Core;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public static class IPageBuilderExtensions
    {
        public static ISiteBuilder<T> CreateSite<T>(this IPageBuilder<T> _, EpiserverFixture fixture)
            where T : PageData => new SiteBuilder<T>(fixture).CreateSite();

    }
}
