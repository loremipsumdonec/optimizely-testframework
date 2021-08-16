using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Globalization;

namespace Lorem.Test.Framework.Optimizely.CMS.Commands
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

        public Action<SiteDefinition> Build { get; set; }

        public SiteDefinition Execute()
        {
            var site = CreateSiteDefinition();
            Build?.Invoke(site);
            Save(site);

            AddHosts(site);
            Save(site);

            return site;
        }

        private void Save(SiteDefinition site)
        {
            _repository.Save(site);
        }

        private SiteDefinition CreateSiteDefinition()
        {
            return new SiteDefinition()
            {
                Name = Name,
                SiteUrl = Url,
                StartPage = StartPage
            };
        }

        private void AddHosts(SiteDefinition site)
        {
            site.Hosts[0].Language = Language;
            site.Hosts.Add(new HostDefinition()
            {
                Name = "*",
                Language = Language
            });
        }
    }
}
