using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using Lorem.Models.Blocks;
using Lorem.Models.Media;
using Lorem.Models.Pages;
using Lorem.Testing.Optimizely.CMS.Builders;
using Lorem.Testing.Optimizely.CMS.Commands;
using Lorem.Testing.Optimizely.CMS.Test.Services;
using Lorem.Testing.Optimizely.CMS.Utility;
using System;
using System.IO;
using System.Linq;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test.Builders
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
        public void ClearContents_AllPagesDeleted()
        {
            var repository = Fixture.GetInstance<IContentRepository>();
            Fixture.Create<StartPage>().Create<ArticlePage>();

            Assert.Single(repository.GetChildren<StartPage>(ContentReference.RootPage));

            var command = new ClearContents();
            command.Clear();

            
            Assert.Empty(repository.GetChildren<StartPage>(ContentReference.RootPage));
        }

        [Fact]
        public void ClearContents_AllBlocksDeleted()
        {
            var repository = Fixture.GetInstance<IContentRepository>();

            Fixture.CreateBlock<HeroBlock>();
            Assert.Single(repository.GetChildren<HeroBlock>(ContentReference.GlobalBlockFolder));

            var command = new ClearContents();
            command.Clear();

            Assert.Empty(repository.GetChildren<HeroBlock>(ContentReference.GlobalBlockFolder));
        }

        [Fact]
        public void ClearContents_AllMediasDeleted()
        {
            var repository = Fixture.GetInstance<IContentRepository>();

            Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom());

            Assert.Single(repository.GetChildren<ImageFile>(ContentReference.GlobalBlockFolder));

            var command = new ClearContents();
            command.Clear();

            Assert.Empty(repository.GetChildren<ImageFile>(ContentReference.GlobalBlockFolder));
        }

        [Fact]
        public void ClearContents_AllMediasFilesDeleted()
        {
            var repository = Fixture.GetInstance<IContentRepository>();
            var registry = Fixture.GetInstance<IBlobProviderRegistry>();
            var provider = (FileBlobProvider)registry.GetProvider(new Uri("//default"));

            Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom());

            Assert.Single(Directory.EnumerateDirectories(provider.Path));

            var command = new ClearContents();
            command.Clear();

            Assert.Empty(Directory.EnumerateDirectories(provider.Path));
        }

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
