using EPiServer.Core;
using Lorem.Testing.Optimizely.CMS.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public class FixtureBuilder<T>
        : IFixtureBuilder<T>
    {
        public FixtureBuilder(EpiserverFixture fixture)
        {
            Fixture = fixture;
        }

        public FixtureBuilder(EpiserverFixture fixture, IEnumerable<IContent> latest)
        {
            Fixture = fixture;

            Fixture.Add(latest);
        }

        public EpiserverFixture Fixture { get; }

        public IFixtureBuilder<T> Pick(int total = 1)
        {
            return new FixtureBuilder<T>(Fixture, Fixture.Latest.PickRandom(total));
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Fixture.Latest.Select(l => (T)l).ToList().GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Fixture.Latest.Select(l => (T)l).ToList().GetEnumerator();
        }
    }
}
