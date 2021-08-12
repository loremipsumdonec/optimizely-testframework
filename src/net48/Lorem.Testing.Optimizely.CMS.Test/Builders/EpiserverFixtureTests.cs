using EPiServer.Core;
using Lorem.Models.Pages;
using Lorem.Testing.Optimizely.CMS.Builders;
using Lorem.Testing.Optimizely.CMS.Test.Services;
using System.Linq;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test.Builders
{
    [Collection("Default")]
    [Trait("verification", "required")]
    public class EpiserverFixtureTests
    {
        public EpiserverFixtureTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
        }

        public DefaultEpiserverFixture Fixture { get; set; }
    
        [Fact]
        public void Reset_NextPageHasRootPageAsParent()
        {
            var firstPage = Fixture.Create<StartPage>().First();
            var nextPage = Fixture.Create<StartPage>().Last();

            Assert.Equal(ContentReference.RootPage, firstPage.ParentLink);
            Assert.Equal(ContentReference.RootPage, nextPage.ParentLink);
        }

        [Fact]
        public void Reset_AfterCreatedASite_NextPageHasSiteStartPageAsParent()
        {
            var startPage = Fixture.CreateSite<StartPage>().First();
            var nextPage = Fixture.Create<ArticlePage>().Last();

            Assert.Equal(startPage.ContentLink.ToReferenceWithoutVersion(), nextPage.ParentLink);
        }
    }
}
