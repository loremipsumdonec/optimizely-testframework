using System.Collections.Generic;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public interface IFixtureBuilder
    {
        Fixture Fixture { get; }
    }

    public interface IFixtureBuilder<T> 
        : IFixtureBuilder, IEnumerable<T>
    {
        IFixtureBuilder<T> Pick(int total = 1);
    }
}
