using EPiServer.Core;
using Lorem.Testing.Optimizely.CMS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public class ContentBuilder<T>
        : FixtureBuilder<T>, IContentBuilder<T> where T: IContentData
    {
        private List<IContent> _contents = new List<IContent>();

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
            foreach (var latest in Fixture.Latest)
            {
                var command = new UpdateContent(latest);
                command.Build = p =>
                {
                    IVersionable content = (IVersionable)p;
                    content.StartPublish = DateTime.Now.AddSeconds(-2);
                    content.StopPublish = null;
                };

                Add(command.Execute());
            }

            return new ContentBuilder<T>(Fixture, _contents);
        }

        public IContentBuilder<T> Expire()
        {
            foreach(var latest in Fixture.Latest) 
            {
                var command = new UpdateContent(latest);
                command.Build = p =>
                {
                    IVersionable content = (IVersionable)p;
                    content.StartPublish = DateTime.Now.AddSeconds(-2);
                    content.StopPublish = DateTime.Now.AddSeconds(-1);
                };

                Add(command.Execute());
            }

            return new ContentBuilder<T>(Fixture, _contents);
        }

        public IContentBuilder<T> Delete()
        {
            foreach(var latest in Fixture.Latest)
            {
                var command = new DeleteContent(latest, false);
                var content = command.Execute();

                Add(content);
            }

            return new ContentBuilder<T>(Fixture, _contents);
        }

        public IContentBuilder<T> Move(ContentReference destination)
        {
            foreach (var latest in Fixture.Latest)
            {
                var command = new MoveContent(latest, destination);
                var content = command.Execute();

                Add(content);
            }

            return new ContentBuilder<T>(Fixture, _contents);
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

        private void Add(IContent content)
        {
            if(content == null)
            {
                return;
            }

            _contents = _contents.Where(p => !p.ContentGuid.Equals(content.ContentGuid)).ToList();
            _contents.Add(content);
        }
    }
}
