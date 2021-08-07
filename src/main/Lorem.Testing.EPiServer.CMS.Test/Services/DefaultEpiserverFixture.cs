using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.Test.Services
{
    public class DefaultEpiserverFixture
        : EpiserverFixtureWithFactory<Startup>
    {
        public DefaultEpiserverFixture(EpiserverEngine<Startup> engine)
        {
            Engine = engine;
            Engine.Start();

            Register("episerver.site.name", "Lorem");
            Register("episerver.site.url", new Uri("http://localhost:65099/"));

            Cultures.Add(CultureInfo.GetCultureInfo("en"));
        }

        public IEnumerable<CultureInfo> GetCmsCultures()
        {
            var repository = ServiceLocator.Current.GetInstance<ILanguageBranchRepository>();
            
            return repository.ListAll()
                    .Where(b => !b.Culture.Equals(CultureInfo.InvariantCulture))
                    .Select(b => b.Culture);
        }
    }
}
