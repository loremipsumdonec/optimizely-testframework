using EPiServer;
using EPiServer.Core;
using Lorem.Models.Pages;
using Lorem.Testing.Optimizely.CMS.Builders;
using Lorem.Testing.Optimizely.CMS.Test.Services;
using Moq;
using System.IO;
using System.Linq;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test
{
    [Collection("Default")]
    public class FixtureNestedContextTests
    {
        public FixtureNestedContextTests(DefaultEngine engine)
        {
            Fixture = new DefaultFixture(engine);
        }

        public DefaultFixture Fixture { get; set; }

        [Fact]
        public void CreateNestedContext_WithUsing_GetChildrenAfterDispose()
        {
            var repository = new Mock<IContentRepository>();
            repository.Setup(r => r.GetChildren<IContent>(It.IsAny<ContentReference>()))
                .Throws(new FileNotFoundException("Only for testing"));

            Fixture.Create<StartPage>().CreateMany<ArticlePage>(2);
            var startPage = Fixture.Get<StartPage>().First();

            using (var context = Fixture.CreateNestedContext())
            {
                context.Container.Configure(_ => _.For<IContentRepository>().Use(repository.Object));

                var r = Fixture.GetInstance<IContentRepository>();
                Assert.Throws<FileNotFoundException>(
                    () => r.GetChildren<IContent>(startPage.ContentLink)
                );
            }

            var rr = Fixture.GetInstance<IContentRepository>();
            var pages = rr.GetChildren<IContent>(startPage.ContentLink);

            Assert.Equal(2, pages.Count());
        }
    }
}
