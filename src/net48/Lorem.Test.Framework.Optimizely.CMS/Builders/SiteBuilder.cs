using EPiServer.Core;
using EPiServer.Web;
using Lorem.Test.Framework.Optimizely.CMS.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public class SiteBuilder<T>
        : FixtureBuilder<T>, ISiteBuilder<T> where T : PageData
    {
        public SiteBuilder(Fixture fixture)
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

            var builders = Fixture.GetBuilders<SiteDefinition>();

            var command = new CreateSite(
                "localhost",
                new Uri("http://localhost"),
                startPage.ContentLink,
                Fixture.Cultures[0]
            )
            {
                Build = CreateBuild()
            };

            Fixture.Site = command.Execute();

            return this;
        }

        public ISiteBuilder<T> CreateSite(string name, string url)
        {
            Enable(Fixture.Cultures);

            var startPage = Fixture.Latest
                .Where(p => p is PageData)
                .Select(p => (PageData)p)
                .LastOrDefault();

            if (startPage == null)
            {
                throw new InvalidOperationException("Could not find a page to be used as start page");
            }

            var command = new CreateSite(
                name,
                new Uri(url),
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

        private Action<object> CreateBuild()
        {
            return p =>
            {
                foreach (var builder in Fixture.GetBuilders<SiteDefinition>())
                {
                    builder.Invoke(p);
                }
            };
        }
    }
}
