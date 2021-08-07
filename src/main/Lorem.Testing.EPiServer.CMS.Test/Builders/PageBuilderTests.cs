using EPiServer.Core;
using Lorem.Models.Pages;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Test.Services;
using Lorem.Testing.EPiServer.CMS.Utility;
using System;
using System.Linq;
using Xunit;

namespace Lorem.Testing.EPiServer.CMS.Test.Builders
{
    [Collection("Default")]
    public class PageBuilderTests
    {
        public PageBuilderTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
        }

        public DefaultEpiserverFixture Fixture { get; set; }

        [Fact]
        public void Create_NoCulturesInFixture_ThrowInvalidOperationException()
        {
            Fixture.Cultures.Clear();

            Assert.Throws<InvalidOperationException>(() => Fixture.Create<StartPage>());
        }

        [Fact]
        public void Create_WithCulture_PageHasSameCultureAsLanguage() 
        {
            Fixture.Create<StartPage>();

            Assert.Equal(
                Fixture.Cultures[0],
                ((PageData)Fixture.Latest.First()).Language
            );
        }

        [Fact]
        public void Create_WithMultipleCultures_PageHasAllCulturesAsExistingLanguages()
        {
            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            Fixture.Create<StartPage>();

            Assert.Equal(
                Fixture.Cultures.OrderBy(c=> c.Name),
                ((PageData)Fixture.Latest.First()).ExistingLanguages.OrderBy(c=> c.Name)
            );
        }

        [Fact]
        public void Create_NoBuildAction_PageExistsInLatest()
        {
            Fixture.Create<StartPage>();

            Assert.Single(Fixture.Latest.Where(s => s is StartPage));
        }

        [Fact]
        public void Create_NoBuildAction_PageIsPublished()
        {
            Fixture.Create<StartPage>();

            Assert.IsPublished(Fixture.Latest);
        }

        [Fact]
        public void Create_WithBuildAction_PageIsPublished()
        {
            Fixture.Create<StartPage>(p=> p.Heading = nameof(Create_WithBuildAction_PageIsPublished));

            Assert.IsPublished(Fixture.Latest);
            Assert.Equal(
                nameof(Create_WithBuildAction_PageIsPublished),
                ((StartPage)Fixture.Latest.First()).Heading
            );
        }
    
        [Fact]
        public void CreateMany_WithNoBuildAction_PagesExistsInLatest()
        {
            int total = 3;
            Fixture.CreateMany<StartPage>(total);

            Assert.Equal(
                total,
                Fixture.Latest.Where(s => s is StartPage).Count()
            );
        }

        [Fact]
        public void CreateMany_WithMultipleCultures_PageHasAllCulturesAsExistingLanguages()
        {
            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            Fixture.CreateMany<StartPage>(3);

            foreach(PageData page in Fixture.Latest)
            {
                Assert.Equal(
                    Fixture.Cultures.OrderBy(c => c.Name),
                    page.ExistingLanguages.OrderBy(c => c.Name)
                );
            }
        }
    
        [Fact]
        public void CreatePath_WithNoBuildAction_HasCreatedAPathWithPages()
        {
            int depth = 3;

            Fixture.CreatePath<StartPage>(depth);

            Assert.Equal(depth, Fixture.Contents.Count());

            for(int index = depth - 1; index > 0; index--)
            {
                Assert.True(
                    Fixture.Contents[index].ParentLink.Equals(Fixture.Contents[index - 1].ContentLink, true)
                );
            }
        }

        [Fact]
        public void CreatePath_WithMultipleCultures_PageHasAllCulturesAsExistingLanguages()
        {
            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            Fixture.CreatePath<StartPage>(3);

            foreach (PageData page in Fixture.Contents)
            {
                Assert.Equal(
                    Fixture.Cultures.OrderBy(c => c.Name),
                    page.ExistingLanguages.OrderBy(c => c.Name)
                );
            }
        }
    }
}
