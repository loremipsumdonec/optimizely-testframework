using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Globalization;

namespace Lorem.Testing.Optimizely.CMS.Commands
{
    public class CreateSite
    {
        private readonly ISiteDefinitionRepository _repository;

        public CreateSite(string name, Uri url, ContentReference startPage, CultureInfo language)
            : this(
                  name,
                  url,
                  startPage,
                  language,
                  ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>()
            )
        {
        }

        public CreateSite(
            string name,
            Uri url,
            ContentReference startPage,
            CultureInfo language,
            ISiteDefinitionRepository repository
        )
        {
            StartPage = startPage;
            Language = language;
            Name = name;
            Url = url;
            _repository = repository;
        }

        public string Name { get; set; }

        public Uri Url { get; set; }

        public ContentReference StartPage { get; set; }

        public CultureInfo Language { get; set; }

        public SiteDefinition Execute()
        {
            var site = CreateSiteDefinition();
            AddHosts(site);

            return site;
        }

        private SiteDefinition CreateSiteDefinition()
        {
            var site = new SiteDefinition()
            {
                Name = Name,
                SiteUrl = Url,
                StartPage = StartPage
            };

            _repository.Save(site);

            return site;
        }

        private void AddHosts(SiteDefinition site)
        {
            site.StartPage = StartPage;
            site.Hosts[0].Language = Language;
            site.Hosts.Add(new HostDefinition()
            {
                Name = "*",
                Language = Language
            });

            _repository.Save(site);
        }
    }
}
