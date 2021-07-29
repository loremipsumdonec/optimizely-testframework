using EPiServer;
using EPiServer.Core;
using Lorem.Models.Pages;
using Lorem.Test.Services;
using System.Linq;
using Xunit;

namespace Lorem.Test
{
    [Collection("Episerver")]
    public class ExampleApiForTestFramework
    {
        public ExampleApiForTestFramework(EpiserverFixture fixture)
        {
            Fixture = fixture;
        }

        public EpiserverFixture Fixture { get; set; }

        [Fact]
        public void GetChildren_OneChild_HasOneChild()
        {
            Fixture.Create<StartPage>();

            var repository = Fixture.GetInstance<IContentRepository>();
            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);

            Assert.Single(pages);
        }

        [Fact]
        public void GetChildren_CreateManyChildren_HasExpectedChildren()
        {
            int expected = 10;

            var pages = Fixture.Create<StartPage>()
                .CreateMany<ArticlePage>(expected);

            var repository = Fixture.GetInstance<IContentRepository>();
            var children = repository.GetChildren<ArticlePage>(pages.First().ContentLink);

            Assert.Equal(expected, children.Count());
        }
    }
}
