using EPiServer.Core;
using Lorem.Models.Pages;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Test.Services;
using Lorem.Testing.EPiServer.CMS.Utility;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
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
        public void Create_IsReadOnlyIsFalse()
        {
            var startPage = Fixture.Create<StartPage>().Last();
            Assert.False(startPage.IsReadOnly);
        }

        [Fact]
        public void CreateMany_IsReadOnlyIsFalse()
        {
            foreach(var page in Fixture.CreateMany<StartPage>(3))
            {
                Assert.False(page.IsReadOnly);
            }
        }

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
        public void Create_WhenPageHasRequired_ThrowsValidationException()
        {
            Assert.Throws<ValidationException>(
                () => Fixture.Create<ArticlePageWithRequired>()
            );
        }

        [Fact]
        public void Create_WhenPageHasRequired_BuildIsInvoked()
        {
            bool buildIsInvoked = false;

            Assert.Throws<ValidationException>(
                () => Fixture.Create<ArticlePageWithRequired>(_ => buildIsInvoked = true)
            );

            Assert.True(buildIsInvoked);
        }

        [Fact]
        public void Create_WithBuildAction_PageIsPublished()
        {
            string heading = IpsumGenerator.Generate(2, 5, false);

            Fixture.Create<StartPage>(p=> p.Heading = heading);

            Assert.IsPublished(Fixture.Latest);
            Assert.Equal(
                heading,
                ((StartPage)Fixture.Latest.First()).Heading
            );
        }

        [Fact]
        public void Create_ChainCreate_LastPageIsChildOfFirst()
        {
            var articlePage = Fixture.Create<StartPage>()
                .Create<ArticlePage>().First();

            Assert.Equal(
                Fixture.Contents[0].ContentLink.ToReferenceWithoutVersion(),
                articlePage.ParentLink
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
    
        [Fact]
        public void Update_WithLatest_LatestPageIsUpdated()
        {
            string expected = "Updated";

            Fixture.Create<StartPage>(p => p.Heading = IpsumGenerator.Generate(2,3,false));
            Fixture.Update<StartPage>(p => p.Heading = "Updated");

            Assert.Equal(
                expected,
                ((StartPage)Fixture.Latest.First()).Heading
            );
        }

        [Fact]
        public void Update_AfterCreatedMany_HasAccessToLatestPagesCreated()
        {
            int expected = 5;

            Fixture.Create<StartPage>()
                .CreateMany<ArticlePage>(expected)
                .Update<StartPage>(
                    (_, articles) => Assert.Equal(expected, articles.Count())
                );
        }

        [Fact]
        public void Update_AfterCreate_LastCreatedPageStillInLatest()
        {
            Fixture.Create<StartPage>()
                .Create<ArticlePage>()
                .Update<StartPage>(
                    (p, _) => p.Heading = "Updated"
                );

            Assert.True(Fixture.Latest[0] is ArticlePage);
        }

        [Fact]
        public void Update_AfterCreatedMany_LastCreatedPagesStillLatest() 
        {
            int expected = 5;

            Fixture.Create<StartPage>()
                .CreateMany<ArticlePage>(expected)
                .Update<StartPage>((p, _) => p.Heading = "Updated");

            Assert.Equal(expected, Fixture.Latest.Count());
        }

        [Fact]
        public void Update_AfterCreatedManyWithMultipleCultures_PagesHasSameCulture()
        {
            int expected = 5;

            Fixture.Cultures.Clear();
            Fixture.Cultures.AddRange(
                Fixture.GetCmsCultures().PickRandom(4)
            );

            List<CultureInfo> cultures = new List<CultureInfo>(Fixture.Cultures);

            Fixture.Create<StartPage>()
                .CreateMany<ArticlePage>(expected)
                .Update<StartPage>(
                    (page, articles) => {

                        cultures.Remove(page.Language);

                        foreach(var article in articles)
                        {
                            Assert.Equal(page.Language, article.Language);
                        }
                    }
                );

            Assert.Empty(cultures);
        }
    }
}
