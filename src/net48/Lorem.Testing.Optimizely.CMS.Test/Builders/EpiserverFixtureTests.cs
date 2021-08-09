using EPiServer.Core;
using Lorem.Models.Pages;
using Lorem.Testing.Optimizely.CMS.Builders;
using Lorem.Testing.Optimizely.CMS.Test.Services;
using Lorem.Testing.Optimizely.CMS.Utility;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test.Builders
{
    [Collection("Default")]
    public class EpiserverFixtureTests
    {
        public EpiserverFixtureTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
        }

        public DefaultEpiserverFixture Fixture { get; set; }
    }
}
