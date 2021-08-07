using EPiServer.Core;
using Lorem.Testing.EPiServer.CMS.Commands;
using Lorem.Testing.EPiServer.CMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.Builders
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

            var command = new UploadFile(
                IpsumGenerator.Generate(3, false).Replace(" ", "_"),
                file,
                Fixture.GetContentType(typeof(TMediaType)),
                GetParent()
            );

            if (build != null)
            {
                command.Build = p => build.Invoke((TMediaType)p);
            }

            var media = command.Execute();
            _medias.Add(media);

            return new MediaBuilder<TMediaType>(Fixture, _medias);
        }

        private ContentReference GetParent()
        {
            ContentReference parent = ContentReference.GlobalBlockFolder;

            if (Fixture.Site != null)
            {
                parent = Fixture.Site.SiteAssetsRoot;
            }

            var page = Fixture.Latest.LastOrDefault(p => p is PageData);

            if (page != null)
            {
                parent = page.ContentLink;
            }

            return parent;
        }
    }
}
