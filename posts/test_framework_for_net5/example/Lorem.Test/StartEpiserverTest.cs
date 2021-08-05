using EPiServer;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Lorem.Test
{
    public class StartEpiserverTest
    {
        [Fact]
        public void StartEpiserver_FirstTestCase_IContentRepositoryIsNotNull()
        {
            var factory = new WebApplicationFactory<Startup>();
            
            IContentRepository repository = factory.Services.GetInstance<IContentRepository>();
            Assert.NotNull(repository);
        }
    }
}
