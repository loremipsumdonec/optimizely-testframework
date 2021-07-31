using Lorem.Testing.EPiServer.CMS;
using System;
using System.Globalization;

namespace Alloy.Test.Services
{
    public class DefaultEpiserverFixture
        : EpiserverFixture
    {
        public DefaultEpiserverFixture(IEpiserverEngine engine)
        {
            Engine = engine;
            Engine.Start();

            Register("episerver.site.name", "lorem");
            Register("episerver.site.url", new Uri("http://localhost:57273/"));
            Register("episerver.site.language", CultureInfo.GetCultureInfo("en"));

            CreateDefaultUser();
        }
    }
}
