using EPiServer.Core;
using Lorem.Models.Blocks;
using Lorem.Models.Media;
using Lorem.Models.Pages;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Test.Services;
using Lorem.Testing.EPiServer.CMS.Utility;
using System.Linq;
using Xunit;

namespace Lorem.Testing.EPiServer.CMS.Test.Builders
{
    [Collection("Default")]
    public class ContentBuilderTests
    {
        public ContentBuilderTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
            Resources = new DefaultResources();
        }

        public DefaultEpiserverFixture Fixture { get; set; }

        public Resources Resources { get; set; }

        [Fact]
        public void Delete_WithPage_PageInRecycleBin()
        {
            var startPage = Fixture.Create<StartPage>().Delete().First();

            Assert.Equal(ContentReference.WasteBasket, startPage.ParentLink);
        }

        [Fact]
        public void Move_WithPage_PageMovedBackToRootPage()
        {
            var startPage = Fixture.Create<StartPage>()
                .Delete()
                .Move(ContentReference.RootPage)
                .First();

            Assert.Equal(ContentReference.RootPage, startPage.ParentLink);
        }

        [Fact]
        public void Expire_WithPublishedPage_PageExpired()
        {
            var startPage = Fixture.Create<StartPage>()
                .Expire()
                .First();

            Assert.IsExpired(startPage);
        }

        [Fact]
        public void Publish_WithExpiredPage_PageIsPublished()
        {
            var startPage = Fixture.Create<StartPage>()
                .Expire()
                .Publish()
                .First();

            Assert.IsPublished(startPage);
        }

        [Fact]
        public void Expire_WithPublishedBlock_BlockExpired()
        {
            var block = Fixture.CreateBlock<HeroBlock>()
                .Expire()
                .First();

            Assert.IsExpired(block);
        }

        [Fact]
        public void Delete_WithBlock_BlockInRecycleBin()
        {
            var block = Fixture.CreateBlock<HeroBlock>()
                .Delete()
                .First();

            Assert.Equal(ContentReference.WasteBasket, block.GetParentLink());
        }

        [Fact]
        public void Move_WithBlock_BlockMovedBackToGlobalBlockFolder()
        {
            var block = Fixture.CreateBlock<HeroBlock>()
                .Delete()
                .Move(ContentReference.GlobalBlockFolder)
                .First();

            Assert.Equal(ContentReference.GlobalBlockFolder, block.GetParentLink());
        }
    
        [Fact]
        public void Publish_WithExpiredBlock_BlockIsPublished()
        {
            var block = Fixture.CreateBlock<HeroBlock>()
                .Expire()
                .Publish()
                .First();

            Assert.IsPublished(block);
        }


        [Fact]
        public void Expire_WithPublishedMedia_MediaExpired()
        {
            var media = Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom())
                .Expire()
                .First();

            Assert.IsExpired(media);
        }

        [Fact]
        public void Delete_WithMedia_MediaInRecycleBin()
        {
            var media = Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom())
                .Delete()
                .First();

            Assert.Equal(ContentReference.WasteBasket, media.ParentLink);
        }

        [Fact]
        public void Move_WithMedia_MediaMovedBackToGlobalBlockFolder()
        {
            var media = Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom())
                .Delete()
                .Move(ContentReference.GlobalBlockFolder)
                .First();

            Assert.Equal(ContentReference.GlobalBlockFolder, media.ParentLink);
        }

        [Fact]
        public void Publish_WithExpiredMedia_MediaIsPublished()
        {
            var media = Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom())
                .Expire()
                .Publish()
                .First();

            Assert.IsPublished(media);
        }

    }
}
