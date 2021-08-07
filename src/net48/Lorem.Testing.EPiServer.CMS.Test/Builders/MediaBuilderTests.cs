using Lorem.Models.Media;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Test.Services;
using Lorem.Testing.EPiServer.CMS.Utility;
using Xunit;

namespace Lorem.Testing.EPiServer.CMS.Test.Builders
{
    [Collection("Default")]
    public class MediaBuilderTests
    {
        public MediaBuilderTests(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
            Resources = new DefaultResources();
        }

        public DefaultEpiserverFixture Fixture { get; set; }

        public Resources Resources { get; set; }

        [Fact]
        public void Upload_ImageWithNoBuildAction_ImageExistsInLatest()
        {
            Fixture.Upload<ImageFile>(Resources.Get("/images").PickRandom());

            Assert.Single(Fixture.Latest);
            Assert.True(Fixture.Latest[0] is ImageFile);
        }
    }
}
