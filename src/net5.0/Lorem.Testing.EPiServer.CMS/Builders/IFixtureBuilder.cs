using System.Collections.Generic;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public interface IFixtureBuilder
    {
        EpiserverFixture Fixture { get; }
    }

    public interface IFixtureBuilder<T> 
        : IFixtureBuilder, IEnumerable<T>
    {
        IFixtureBuilder<T> Pick(int total = 1);
    }
}
