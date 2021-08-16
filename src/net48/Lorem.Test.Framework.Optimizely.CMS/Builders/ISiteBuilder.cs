using EPiServer.Core;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public interface ISiteBuilder<T>
        : IFixtureBuilder<T> where T : PageData
    {
        ISiteBuilder<T> CreateSite();

        ISiteBuilder<T> CreateSite(string name, string url);
    }
}
