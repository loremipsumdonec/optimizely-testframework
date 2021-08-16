using EPiServer.Web;
using Lorem.Models.Pages;
using Lorem.Test.Framework.Optimizely.CMS.Builders;
using Lorem.Test.Framework.Optimizely.CMS.Commands;
using Lorem.Test.Framework.Optimizely.CMS.Test.Services;
using System;
using System.Linq;
using Xunit;

namespace Lorem.Test.Framework.Optimizely.CMS.Test.Builders
{
    [Collection("Default")]
    [Trait("verification", "required")]
    public class SiteBuilderTests
    {
        public SiteBuilderTests(DefaultEngine engine)
        {
            Fixture = new DefaultFixture(engine);
        }

        public DefaultFixture Fixture { get; set; }

        [Theory]
        [InlineData("siteName", "http://site.local")]
        public void CreateSite_WithBuilder_SiteDefinitionCreatedWithInputFromBuilder(string siteName, string siteUrl)
        {
            Fixture.ClearBuilders<SiteDefinition>();
            Fixture.RegisterBuilder<SiteDefinition>(s => {
                s.Name = siteName;
                s.SiteUrl = new Uri(siteUrl);
            });

            Fixture.CreateSite<StartPage>();

            var repository = Fixture.GetInstance<ISiteDefinitionRepository>();
            var site = repository.List().FirstOrDefault(s => s.Name == siteName);

            Assert.NotNull(site);
            Assert.NotNull(site.Hosts.FirstOrDefault(h => h.Url.AbsoluteUri.StartsWith(siteUrl)));
        }

        [Theory]
        [InlineData("siteName", "http://site.local")]
        public void CreateSite_WithInput_SiteDefinitionCreatedWithInput(string siteName, string siteUrl)
        {
            Fixture.CreateSite<StartPage>(
                siteName, siteUrl
            );

            var repository = Fixture.GetInstance<ISiteDefinitionRepository>();
            var site = repository.List().FirstOrDefault(s => s.Name == siteName);
            
            Assert.NotNull(site);
            Assert.NotNull(site.Hosts.FirstOrDefault(h => h.Url.AbsoluteUri.StartsWith(siteUrl)));
        }

        [Theory]
        [InlineData("siteName", "http://site.local")]
        public void CreateSite_WithInputAndHasSiteDefinitionBuilder_BuilderNotInvoked(string siteName, string siteUrl)
        {
            bool invoked = false;

            Fixture.ClearBuilders<SiteDefinition>();
            Fixture.RegisterBuilder<SiteDefinition>(_ => invoked = true);

            Fixture.CreateSite<StartPage>(
                siteName, siteUrl
            );

            Assert.False(invoked);
        }

        [Fact]
        public void CreateSite_WithBuild_BuildInvoked()
        {
            bool buildInvoked = false;

            Fixture.CreateSite<StartPage>(_ => buildInvoked = true);

            Assert.True(buildInvoked);
        }
    
        [Fact]
        public void ClearSites_AllSitesRemoved()
        {
            Fixture.CreateSite<StartPage>("donec", "http://lorem.local");

            var command = new ClearSites();
            command.Clear();

            var repository = Fixture.GetInstance<ISiteDefinitionRepository>();

            Assert.Empty(repository.List());
        }
    }
}
