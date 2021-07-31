using EPiServer.Core;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public interface IContentBuilder
        : IFixtureBuilder
    {
    }

    public interface IContentBuilder<T>
        : IFixtureBuilder<T>, IContentBuilder where T : IContent
    {
        IContentBuilder<T> Publish();

        IContentBuilder<T> Expire();

        IContentBuilder<T> Delete();

        void ForceDelete();

        IContentBuilder<T> Move(ContentReference destination);
    }
}
