using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using Lorem.Models.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS.Test.Services
{
    public class ExploratoryEpiserverFixture
        : EpiserverFixture
    {
        public ExploratoryEpiserverFixture(EpiserverEngine engine)
        {
            Engine = engine;
            Engine.Start();

            Register("episerver.site.name", "Lorem min");
            Register("episerver.site.url", new Uri("http://localhost:61352/"));

            Cultures.Add(CultureInfo.GetCultureInfo("en"));

            RegisterBuilder<StartPage>(p => p.Heading = "Welcome to Lorem minimum");
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
