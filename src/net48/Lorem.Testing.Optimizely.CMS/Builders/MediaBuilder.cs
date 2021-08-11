using EPiServer.Core;
using Lorem.Testing.Optimizely.CMS.Commands;
using Lorem.Testing.Optimizely.CMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public class MediaBuilder<T>
        : FixtureBuilder<T>, IMediaBuilder<T> where T : MediaData
    {
        private readonly List<MediaData> _medias = new List<MediaData>();

        public MediaBuilder(EpiserverFixture fixture)
            : base(fixture)
        {
        }

        public MediaBuilder(EpiserverFixture fixture, IEnumerable<MediaData> medias)
            : base(fixture, medias)
        {
        }

        public IMediaBuilder<TMediaType> Upload<TMediaType>(string file, Action<TMediaType> build = null) where TMediaType : MediaData
        {
            if(!File.Exists(file))
            {
                throw new FileNotFoundException($"could not find file {file}, verify that you have set \"Copy to Output Directory = Copy always\"");
            }

            if(Fixture.Latest.Count > 0)
            {
                return UploadWhenHasLatest(file, build);
            }

            var command = new UploadFile(
               IpsumGenerator.Generate(3, false).Replace(" ", "_"),
               file,
               Fixture.GetContentType(typeof(TMediaType)),
               GetParent()
            );

            command.Build = CreateBuild(build);

            var media = command.Execute();
            _medias.Add(media);

            return new MediaBuilder<TMediaType>(Fixture, _medias);
        }

        private IMediaBuilder<TMediaType> UploadWhenHasLatest<TMediaType>(string file, Action<TMediaType> build) 
            where TMediaType : MediaData
        {
            foreach (var latest in Fixture.Latest)
            {
                var command = new UploadFile(
                   IpsumGenerator.Generate(3, false).Replace(" ", "_"),
                   file,
                   Fixture.GetContentType(typeof(TMediaType)),
                   GetParent(latest)
                );

                command.Build = CreateBuild(build);

                var media = command.Execute();
                _medias.Add(media);
            }

            return new MediaBuilder<TMediaType>(Fixture, _medias);
        }

        private ContentReference GetParent(IContent content = null)
        {
            ContentReference parent = ContentReference.GlobalBlockFolder;

            if (Fixture.Site != null)
            {
                parent = Fixture.Site.SiteAssetsRoot;
            }

            if (content is PageData page)
            {
                parent = page.ContentLink;
            }

            if (content is BlockData block)
            {
                parent = ((IContent)block).ContentLink;
            }

            if (content is MediaData media)
            {
                parent = media.ParentLink;
            }

            return parent;
        }

        private Action<object> CreateBuild<TMediaType>(Action<TMediaType> build)
            where TMediaType : MediaData
        {
            return p =>
            {
                foreach (var builder in Fixture.GetBuilders<TMediaType>())
                {
                    builder.Invoke(p);
                }

                build?.Invoke((TMediaType)p);
            };
        }
    }
}
