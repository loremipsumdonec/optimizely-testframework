using EPiServer.Web;
using Lorem.Models.Pages;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Commands;
using Lorem.Testing.EPiServer.CMS.Test.Services;
using System;
using System.Linq;
using Xunit;

namespace Lorem.Testing.EPiServer.CMS.Test.Builders
{
    [Collection("Default")]
    public class SiteBuilderTests
    {
        public SiteBuilderTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
        }

        public DefaultEpiserverFixture Fixture { get; set; }

        [Fact]
        public void CreateSite_WithPage_HasSiteDefinition()
        {
            string siteName = Fixture.Get<string>("episerver.site.name");
            string siteUrl = Fixture.Get<Uri>("episerver.site.url").AbsoluteUri;

            Fixture.CreateSite<StartPage>(
                siteName, siteUrl
            );

            var repository = Fixture.GetInstance<ISiteDefinitionRepository>();
            var site = repository.List().FirstOrDefault(s => s.Name == siteName);
            
            Assert.NotNull(site);
            Assert.NotNull(site.Hosts.FirstOrDefault(h => h.Url.AbsoluteUri == siteUrl));
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
