using EPiServer.Core;
using Lorem.Models.Blocks;
using Lorem.Models.Pages;
using Lorem.Test.Framework.Optimizely.CMS.Builders;
using Lorem.Test.Framework.Optimizely.CMS.Test.Services;
using Lorem.Test.Framework.Optimizely.CMS.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Lorem.Test.Framework.Optimizely.CMS.Test.Builders
{
    [Collection("Default")]
    [Trait("verification", "required")]
    public class BlockBuilderTests
    {
        public BlockBuilderTests(DefaultEngine engine)
        {
            Fixture = new DefaultFixture(engine);
        }

        public DefaultFixture Fixture { get; set; }

        [Fact]
        public void CreateBlock_NoBuildAction_BlockExistsInLatest()
        {
            Fixture.CreateBlock<HeroBlock>();

            Assert.Single(Fixture.Latest);
            Assert.True(Fixture.Latest[0] is HeroBlock);
        }

        [Fact]
        public void CreateBlock_NoBuildAction_BlockIsPublished()
        {
            Fixture.CreateBlock<HeroBlock>();

            Assert.IsPublished(Fixture.Latest);
        }

        [Fact]
        public void CreateBlock_WithBuildAction_BlockIsPublished()
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
        public void CreateBlock_NoCulturesInFixture_ThrowInvalidOperationException()
        {
            Fixture.Cultures.Clear();

            Assert.Throws<InvalidOperationException>(() => Fixture.CreateBlock<HeroBlock>());
        }

        [Fact]
        public void CreateBlock_WithCulture_PageHasSameCultureAsLanguage()
        {
            Fixture.CreateBlock<HeroBlock>();

            Assert.Equal(
                Fixture.Cultures[0],
                ((ILocale)Fixture.Latest.First()).Language
            );
        }

        [Fact]
        public void CreateBlock_WithMultipleCultures_BlockHasAllCulturesAsExistingLanguages()
        {
            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            Fixture.CreateBlock<HeroBlock>();

            Assert.Equal(
                Fixture.Cultures.OrderBy(c => c.Name),
                ((ILocalizable)Fixture.Latest.First()).ExistingLanguages.OrderBy(c => c.Name)
            );
        }

        [Fact]
        public void CreateBlock_WithBuildActionAndMultipleCultures_BuildInvokedForEachCulture()
        {
            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            var cultures = new List<CultureInfo>(Fixture.Cultures);

            Fixture.CreateBlock<HeroBlock>(b => cultures.Remove(((ILocale)b).Language));

            Assert.Empty(cultures);
        }

        [Fact]
        public void CreateBlock_WhenBlockHasRequired_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(
                () => Fixture.CreateBlock<HeroBlockWithRequired>()
            );
        }

        [Fact]
        public void CreateBlock_WhenBlockHasRequired_BuildIsInvoked()
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
        public void CreateBlock_AfterCreateMany_BlockIsInGlobalBlockFolder()
        {
            var heroBlock = Fixture.CreateMany<StartPage>(2)
                .CreateBlock<HeroBlock>()
                .First();

            Assert.Equal(ContentReference.GlobalBlockFolder, heroBlock.GetParentLink());
        }

        [Fact]
        public void CreateBlock_AfterCreateManyAndSiteExists_BlockIsInSiteAssetsRoot()
        {
            var heroBlock = Fixture.CreateSite<StartPage>()
                .CreateMany<ArticlePage>(2)
                .CreateBlock<HeroBlock>()
                .First();

            Assert.Equal(Fixture.Site.SiteAssetsRoot, heroBlock.GetParentLink());
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

        [Fact]
        public void CreateBlock_WhenSiteBlockHasBuilder_BuilderRun()
        {
            var heading = IpsumGenerator.Generate(1, 4, false);

            Fixture.RegisterBuilder<SiteBlock>(p => p.Heading = heading);

            Fixture.CreateBlock<HeroBlock>(p => Assert.Equal(heading, p.Heading));
        }

        [Fact]
        public void CreateBlock_WhenBlockHasBuilder_BuilderRunFirst()
        {
            var preamble = IpsumGenerator.Generate(12, 14, false);
            Fixture.RegisterBuilder<HeroBlock>(p => p.Preamble = preamble);

            Fixture.CreateBlock<HeroBlock>(p => Assert.Equal(preamble, p.Preamble));
        }
    
        [Fact]
        public void Update_WithPageTypeHasAccessToBlock_PageUpdated() 
        {
            Fixture.Create<StartPage>()
                .CreateBlock<HeroBlock>()
                .Update<StartPage>((p, b) =>
                {
                    p.ContentArea = new ContentArea();

                    p.ContentArea.Items.Add(
                        new ContentAreaItem() { ContentLink = b.GetContentLink() }
                    );
                });

            var startPage = Fixture.Get<StartPage>().First();

            Assert.Single(startPage.ContentArea.Items);
        }
    }
}
