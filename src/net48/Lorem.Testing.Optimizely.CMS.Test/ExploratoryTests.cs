using Lorem.Models.Pages;
using Lorem.Testing.Optimizely.CMS.Builders;
using Lorem.Testing.Optimizely.CMS.Test.Services;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test.Builders
{
    [Collection("Default")]
    [Trait("type", "exploratory")]
    public class ExploratoryTests
    {
        public ExploratoryTests(DefaultEpiserverEngine engine)
        {
            Fixture = new ExploratoryEpiserverFixture(engine);
        }

        public ExploratoryEpiserverFixture Fixture { get; set; }

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
                .Update<StartPage>((p, articlePages) =>
                {
                    p.Heading = p.Language.Name;

                    foreach(var articlePage in articlePages)
                    {
                        p.Add(articlePage);
                    }
                })
                .CreateMany(2);
        }
    }
}
