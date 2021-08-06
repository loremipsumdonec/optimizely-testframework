using Xunit;

namespace Lorem.Test.Services
{
    [CollectionDefinition("Default")]
    public class EpiserverEngineCollectionFixture
        : ICollectionFixture<EpiserverEngineSecondIteration<Startup>>
    {
    }
}
