using Xunit;

namespace Alloy.Test.Services
{
    [CollectionDefinition("Episerver")]
    public class EpiserverEngineCollectionFixture
        : ICollectionFixture<DefaultEpiserverEngine>
    {
    }
}
