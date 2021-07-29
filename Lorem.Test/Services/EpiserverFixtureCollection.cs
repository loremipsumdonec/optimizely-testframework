using Xunit;

namespace Lorem.Test.Services
{
    [CollectionDefinition("Episerver")]
    public class EpiserverFixtureCollectionFixture
        : ICollectionFixture<EpiserverFixture>
    {
    }
}
