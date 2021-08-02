using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using System;
using System.Globalization;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    public class CreateSite
    {
        private readonly ISiteDefinitionRepository _repository;
        private readonly ILanguageBranchRepository _languageBranchRepository;

        public CreateSite(string name, Uri url, ContentReference startPage, CultureInfo language)
            : this(
                  name,
                  url,
                  startPage,
                  language,
                  ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>(),
                  ServiceLocator.Current.GetInstance<ILanguageBranchRepository>()
            )
        {
        }

        public CreateSite(
            string name,
            Uri url,
            ContentReference startPage,
            CultureInfo language,
            ISiteDefinitionRepository repository,
            ILanguageBranchRepository languageBranchRepository
        )
        {
            StartPage = startPage;
            Language = language;
            Name = name;
            Url = url;
            _repository = repository;
            _languageBranchRepository = languageBranchRepository;
        }

        public string Name { get; set; }

        public Uri Url { get; set; }

        public ContentReference StartPage { get; set; }

        public CultureInfo Language { get; set; }

        public SiteDefinition Execute()
        {
            var site = CreateSiteDefinition();
            EnableLanguages();
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

        private void EnableLanguages()
        {
            foreach (var branch in _languageBranchRepository.ListAll())
            {
                if (branch.Culture.Equals(Language))
                {
                    _languageBranchRepository.Enable(Language);
                }
                else if (branch.Enabled)
                {
                    _languageBranchRepository.Disable(branch.Culture);
                }
            }
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
