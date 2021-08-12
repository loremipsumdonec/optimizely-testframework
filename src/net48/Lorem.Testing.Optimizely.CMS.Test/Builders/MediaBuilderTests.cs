using EPiServer;
using EPiServer.Core;
using Lorem.Models.Blocks;
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
    [Trait("verification", "required")]
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
        public void Upload_AfterCreateBlock_ImageIsInBlockAssetsFolder()
        {
            Fixture.CreateBlock<HeroBlock>()
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

        [Fact]
        public void Upload_AfterCreateMany_OneImageForEachPage()
        {
            Fixture.CreateMany<ArticlePage>(3)
                .Upload<ImageFile>(Resources.Get("/images").PickRandom());

            var helper = Fixture.GetInstance<ContentAssetHelper>();
            var loader = Fixture.GetInstance<IContentLoader>();

            foreach(var articlePage in Fixture.Contents.Where(p=> p is ArticlePage))
            {
                var folder = helper.GetAssetFolder(articlePage.ContentLink);

                var images = loader.GetChildren<ImageFile>(
                    folder.ContentLink
                );

                Assert.Single(images);
            }
        }


        [Fact]
        public void Upload_AfterCreateManyWithMultipleCultures_OneImageForEachPage()
        {
            int totalPages = 3;

            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            Fixture.CreateMany<ArticlePage>(totalPages)
                .Upload<ImageFile>(Resources.Get("/images").PickRandom());

            Assert.Equal(totalPages, Fixture.Contents.Where(c => c is ImageFile).Count());
        }

        [Fact]
        public void Upload_AfterCreateManyWithBuildWithPage_HasExpectedPages()
        {
            int totalPages = 3;

            var pages = Fixture.CreateMany<ArticlePage>(3)
                .Upload<ImageFile>(Resources.Get("/images"),
                    (i, p) => p.TopImage = i.ContentLink
                );

            Assert.Equal(totalPages, pages.Count());
        }

        [Fact]
        public void Upload_AfterCreateManyWithBuildWithPagesAndMultipleCultures_SamePageWithDifferentCultureUseSameImage()
        {
            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            var pages = Fixture.CreateMany<ArticlePage>(3)
                .Upload<ImageFile>(Resources.Get("/images"),
                    (i, p) => p.TopImage = i.ContentLink
                );

            var loader = Fixture.GetInstance<IContentLoader>();

            foreach (var page in pages)
            {
                foreach(var culture in Fixture.Cultures)
                {
                    var pageWithCulture = loader.Get<ArticlePage>(page.ContentLink, culture);
                    Assert.Equal(
                        page.TopImage.ToReferenceWithoutVersion(), 
                        pageWithCulture.TopImage.ToReferenceWithoutVersion()
                    );
                }
            }
        }

        [Fact]
        public void Upload_AfterCreateMany_BuildHasAccessToPageAndImageAndOneImageForEachPage()
        {
            var pages = Fixture.CreateMany<ArticlePage>(3)
                .Upload<ImageFile>(Resources.Get("/images"),
                    (i, p) => p.TopImage = i.ContentLink
                );

            var images = Fixture.Contents.Where(c => c is ImageFile).ToList();

            foreach(var page in pages)
            {
                Assert.NotNull(page.TopImage);
                Assert.Null(
                    pages.FirstOrDefault(
                        p => p.ContentGuid != page.ContentGuid && p.TopImage.Equals(page.TopImage, true)
                    )
                );
            }
        }
    }
}
