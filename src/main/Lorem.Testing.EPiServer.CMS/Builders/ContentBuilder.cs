using EPiServer.Core;
using Lorem.Testing.EPiServer.CMS.Commands;
using System.Collections.Generic;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public class ContentBuilder<T>
        : FixtureBuilder<T>, IContentBuilder<T> where T: IContent
    {
        public ContentBuilder(EpiserverFixture fixture)
            : base(fixture)
        {
        }

        public ContentBuilder(EpiserverFixture fixture, IEnumerable<IContent> latest)
            : base(fixture, latest)
        {
        }

        public IContentBuilder<T> Publish()
        {
            return this;
        }

        public IContentBuilder<T> Expire()
        {
            return this;
        }

        public IContentBuilder<T> Delete()
        {
            foreach(var latest in Fixture.Latest)
            {
                var command = new DeleteContent(latest, false);
                command.Execute();
            }

            return this;
        }

        public IContentBuilder<T> Move(ContentReference destination)
        {
            foreach (var latest in Fixture.Latest)
            {
                var command = new DeleteContent(latest, false);
                command.Execute();
            }

            return this;
        }

        public void ForceDelete()
        {
            foreach (var latest in Fixture.Latest)
            {
                var command = new DeleteContent(latest, false);
                command.Execute();

                Fixture.Contents.Remove(latest);
            }

            Fixture.Latest.Clear();
        }
    }
}
