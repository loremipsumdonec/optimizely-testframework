using EPiServer.Core;
using Lorem.Models.Media;
using Lorem.Models.Pages;
using Lorem.Testing.Optimizely.CMS.Builders;
using Lorem.Testing.Optimizely.CMS.Test.Services;
using Lorem.Testing.Optimizely.CMS.Utility;
using System.Linq;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test.Builders
{
    [Collection("Default")]
    public class MediaBuilderTests
    {
        public MediaBuilderTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
            Resources = new DefaultResources();
        }

        public DefaultEpiserverFixture Fixture { get; set; }

        public Resources Resources { get; set; }

        [Fact]
        public void Upload_IsReadOnlyIsFalse()
        {
            Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom());
            var image = (MediaData)Fixture.Latest[0];
            Assert.False(image.IsReadOnly);
        }

        [Fact]
        public void Upload_ImageWithNoBuildAction_ImageExistsInLatest()
        {
            Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom());

            Assert.Single(Fixture.Latest);
            Assert.True(Fixture.Latest[0] is ImageFile);
        }

        [Fact]
        public void Upload_ImageWithNoBuildAction_ImageIsPublished()
        {
            Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom());

            Assert.IsPublished(Fixture.Latest);
        }

        [Fact]
        public void Upload_WithBuildAction_ImageIsPublished()
        {
            string heading = IpsumGenerator.Generate(2, 5, false);

            Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom(), p => p.Heading = heading);

            Assert.IsPublished(Fixture.Latest);
            Assert.Equal(
                heading,
                ((ImageFile)Fixture.Latest.First()).Heading
            );
        }

        [Fact]
        public void Upload_AfterCreatePage_ImageIsInPageAssetsFolder()
        {
            Fixture.Create<StartPage>()
                .Upload<ImageFile>(Resources.Get("/images").PickRandom());

            var helper = Fixture.GetInstance<ContentAssetHelper>();
            var contentFolder = helper.GetAssetFolder(Fixture.Contents[0].ContentLink);

            Assert.Equal(contentFolder.ContentLink, Fixture.Contents[1].ParentLink);
        }

        [Fact]
        public void Upload_ChainUploadAfterCreatePage_ImagesHasSameParent()
        {
            Fixture.Create<StartPage>()
                .Upload<ImageFile>(Resources.Get("/images").PickRandom())
                .Upload<ImageFile>(Resources.Get("/images").PickRandom());

            Assert.Equal(
                Fixture.Contents[1].ParentLink,
                Fixture.Contents[2].ParentLink
            );
        }

        [Fact]
        public void Upload_WhenSiteBlockHasBuilder_BuilderRun()
        {
            var heading = IpsumGenerator.Generate(1, 4, false);

            Fixture.RegisterBuilder<SiteImageFile>(p => p.Heading = heading);

            Fixture.Upload<ImageFile>(
                Resources.Get("/images").PickRandom(),
                p => Assert.Equal(heading, p.Heading)
            );
        }

        [Fact]
        public void Upload_WhenPageHasBuilder_BuilderRunFirst()
        {
            var alt = IpsumGenerator.Generate(12, 14, false);
            Fixture.RegisterBuilder<ImageFile>(p => p.Alt = alt);

            Fixture.Upload<ImageFile>(
                Resources.Get("/images").PickRandom(),
                p => Assert.Equal(alt, p.Alt)
            );
        }
    }
}
