using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
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
        public void ReplaceServiceWith_WithUsing_GetChildrenAfterDispose()
        {
            var mock = new Mock<IContentRepository>();

            mock.Setup(
                r => r.GetChildren<StartPage>(It.IsAny<ContentReference>())
            ).Throws(new FileNotFoundException("Only for testing"));

            Fixture.Create<StartPage>();

            using (Fixture.ReplaceServiceWith(mock.Object))
            {
                var testDoubleRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

                Assert.Throws<FileNotFoundException>(
                    () => testDoubleRepository.GetChildren<StartPage>(ContentReference.RootPage)
                );
            }

            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);

            Assert.Single(pages);
        }
    }
}
