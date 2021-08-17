using Lorem.Test.Framework.Optimizely.CMS.Test.Services;
using System.Configuration;
using Xunit;

namespace Lorem.Test.Framework.Optimizely.SearchAndNavigation.Test
{
    [Collection("Default")]
    /*[Trait("verification", "required")]*/
    public class SearchAndNavigationConfigurationTests
    {
        public SearchAndNavigationConfigurationTests(DefaultEngine engine)
        {
            Fixture = new DefaultFixture(engine);
        }

        public DefaultFixture Fixture { get; set; }

        [Fact]
        public void SearchAndNavigationModule_ConfigurationManagerHasConnectionString() 
        {
            Assert.NotNull(ConfigurationManager.ConnectionStrings["EPiServerDB"]);
        }
    }
}
