using System.Collections.Generic;

namespace Lorem.Testing.EPiServer.CMS.Builders
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
