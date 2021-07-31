using EPiServer.Core;
using Lorem.Testing.EPiServer.CMS.Commands;
using Lorem.Testing.EPiServer.CMS.Utility;
using System;
using System.Collections.Generic;
using System.IO;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public class MediaBuilder<T>
        : FixtureBuilder<T>, IMediaBuilder<T> where T : MediaData
    {
        public MediaBuilder(EpiserverFixture fixture)
            : base(fixture)
        {
        }

        public MediaBuilder(EpiserverFixture fixture, IEnumerable<MediaData> medias)
            : base(fixture, medias)
        {
        }

        public IMediaBuilder<T> Upload<TMediaType>(string file, Action<TMediaType> build = null) where TMediaType : MediaData
        {
            if(Fixture.Site == null)
            {
                throw new InvalidOperationException("you need to create a site before uploading a file");
            }

            if(!File.Exists(file))
            {
                throw new FileNotFoundException($"could not find file {file}, verify that you have set \"Copy to Output Directory = Copy always\"");
            }

            var command = new UploadFile(
                IpsumGenerator.Generate(3, false).Replace(" ", "_"),
                file,
                Fixture.GetContentType(typeof(TMediaType)),
                Fixture.Site.SiteAssetsRoot
            )
            {
                HasAssetFolder = true,
            };

            if (build != null)
            {
                command.Build = p => build.Invoke((TMediaType)p);
            }

            command.Execute();

            return this;
        }
    }
}
