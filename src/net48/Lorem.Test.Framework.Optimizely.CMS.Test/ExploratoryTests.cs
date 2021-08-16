using EPiServer;
using EPiServer.Core;
using Lorem.Models.Media;
using Lorem.Models.Pages;
using Lorem.Test.Framework.Optimizely.CMS.Builders;
using Lorem.Test.Framework.Optimizely.CMS.Test.Services;
using Lorem.Test.Framework.Optimizely.CMS.Utility;
using Moq;
using System.IO;
using System.Linq;
using Xunit;

namespace Lorem.Test.Framework.Optimizely.CMS.Test.Builders
{
    [Collection("Default")]
    [Trait("type", "exploratory")]
    public class ExploratoryTests
    {
        public ExploratoryTests(DefaultEngine engine)
        {
            Fixture = new ExploratoryFixture(engine);
            Resources = new DefaultResources();
        }

        public ExploratoryFixture Fixture { get; set; }

        public Resources Resources { get; set; }

        [Fact]
        public void CreateASimpleSiteForExploratoryTesting()
        {
            Fixture.CreateUser(
                "Administrator",
                "Administrator123!",
                "admin@supersecretpassword.io",
                "WebAdmins", "Administrators"
            );

            Fixture.CreateSite<StartPage>()
                .CreateMany<ArticlePage>(10, p => p.Name = $"{p.Name}-{p.Language.Name}")
                .Upload<ImageFile>(Resources.Get("/images"), (i, p) => p.TopImage = i.ContentLink);

            var repository = Fixture.GetInstance<IContentLoader>();
            Assert.Single(repository.GetChildren<StartPage>(ContentReference.RootPage));

            Fixture.CreateMany<ArticlePage>(2, p => p.Name = $"{p.Name} MANY");
        }
    }
}
