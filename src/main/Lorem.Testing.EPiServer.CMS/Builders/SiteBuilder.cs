using EPiServer.Core;
using Lorem.Testing.EPiServer.CMS.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public class SiteBuilder<T>
        : FixtureBuilder<T>, ISiteBuilder<T> where T : PageData
    {
        public SiteBuilder(EpiserverFixture fixture)
            : base(fixture)
        {
        }

        public ISiteBuilder<T> CreateSite()
        {
            Enable(Fixture.Cultures);

            var startPage = Fixture.Latest
                .Where(p => p is PageData)
                .Select(p => (PageData)p)
                .LastOrDefault();

            if(startPage == null)
            {
                throw new InvalidOperationException("Could not find a page to be used as start page");
            }

            var command = new CreateSite(
                Fixture.Get<string>("episerver.site.name"),
                Fixture.Get<Uri>("episerver.site.url"),
                startPage.ContentLink,
                Fixture.Cultures[0]
            );

            Fixture.Site = command.Execute();

            return this;
        }

        public void Enable(IEnumerable<CultureInfo> cultures)
        {
            foreach(var culture in cultures)
            {
                var command = new UpdateLanguage(culture, true);
                command.Execute();
            }
        }
    }
}
