using EPiServer.Core;
using Lorem.Models.Blocks;
using Lorem.Models.Pages;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Test.Services;
using Lorem.Testing.EPiServer.CMS.Utility;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Xunit;

namespace Lorem.Testing.EPiServer.CMS.Test.Builders
{
    [Collection("Default")]
    public class BlockBuilderTests
    {
        public BlockBuilderTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
        }

        public DefaultEpiserverFixture Fixture { get; set; }

        [Fact]
        public void Create_NoBuildAction_BlockExistsInLatest()
        {
            Fixture.CreateBlock<HeroBlock>();

            Assert.Single(Fixture.Latest);
            Assert.True(Fixture.Latest[0] is HeroBlock);
        }

        [Fact]
        public void Create_NoBuildAction_BlockIsPublished()
        {
            Fixture.CreateBlock<HeroBlock>();

            Assert.IsPublished(Fixture.Latest);
        }

        [Fact]
        public void Create_WithBuildAction_BlockIsPublished()
        {
            string heading = IpsumGenerator.Generate(2, 5, false);

            Fixture.CreateBlock<HeroBlock>(p => p.Heading = heading);

            Assert.IsPublished(Fixture.Latest);
            Assert.Equal(
                heading,
                ((HeroBlock)Fixture.Latest.First()).Heading
            );
        }

        [Fact]
        public void Create_WhenBlockHasRequired_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(
                () => Fixture.CreateBlock<HeroBlockWithRequired>()
            );
        }

        [Fact]
        public void Create_WhenBlockHasRequired_BuildIsInvoked()
        {
            bool buildIsInvoked = false;

            Assert.Throws<ValidationException>(
                () => Fixture.CreateBlock<HeroBlockWithRequired>(_ => buildIsInvoked = true)
            );

            Assert.True(buildIsInvoked);
        }
    
        [Fact]
        public void CreateBlock_AfterCreatePage_BlockIsInPageAssetsFolder()
        {
            var block = Fixture.Create<StartPage>()
                .CreateBlock<HeroBlock>()
                .Last();

            var helper = Fixture.GetInstance<ContentAssetHelper>();
            var contentFolder = helper.GetAssetFolder(Fixture.Contents[0].ContentLink);

            Assert.Equal(contentFolder.ContentLink, block.GetParentLink());
        }

        [Fact]
        public void CreateBlock_ChainCreateBlockAfterCreatePage_BlocksHasSameParent()
        {
            Fixture.Create<StartPage>()
                .CreateBlock<HeroBlock>()
                .CreateBlock<HeroBlock>();

            Assert.Equal(
                Fixture.Contents[1].ParentLink,
                Fixture.Contents[2].ParentLink
            );
        }
    }
}
