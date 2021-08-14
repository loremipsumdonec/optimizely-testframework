using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Lorem.Models.Pages;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS.Test.Services
{
    public class ExploratoryFixture
        : Fixture
    {
        public ExploratoryFixture(IEngine engine)
            : base(engine)
        {
            Cultures.Add(CultureInfo.GetCultureInfo("sv"));
            Cultures.Add(CultureInfo.GetCultureInfo("en"));

            RegisterBuilder<SiteDefinition>(s => {
                s.Name = "Lorem";
                s.SiteUrl = new Uri("http://localhost:65099");
            });

            RegisterBuilder<StartPage>(p => p.Heading = "Welcome to Lorem maximum");

            Start();
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
