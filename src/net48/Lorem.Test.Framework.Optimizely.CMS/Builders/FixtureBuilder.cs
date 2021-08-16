using EPiServer.Core;
using Lorem.Test.Framework.Optimizely.CMS.Utility;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public class FixtureBuilder<T>
        : IFixtureBuilder<T>
    {
        public FixtureBuilder(Fixture fixture)
        {
            Fixture = fixture;
        }

        public FixtureBuilder(Fixture fixture, IEnumerable<IContent> latest)
        {
            Fixture = fixture;

            Fixture.Add(latest);
        }

        public Fixture Fixture { get; }

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
