using EPiServer.Core;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public interface ISiteBuilder
        : IFixtureBuilder
    {
    }

    public interface ISiteBuilder<T>
        : ISiteBuilder where T : PageData
    {
        ISiteBuilder<T> CreateSite();
    }
}
