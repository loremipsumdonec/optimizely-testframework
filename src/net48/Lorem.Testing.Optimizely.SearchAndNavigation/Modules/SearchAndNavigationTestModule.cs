﻿using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using Lorem.Testing.Optimizely.CMS.Commands;
using Lorem.Testing.Optimizely.SearchAndNavigation.Configuration;
using System.Collections.Generic;
using System.Configuration;

namespace Lorem.Testing.Optimizely.CMS.Modules
{
    public class SearchAndNavigationTestModule
        : ITestModule
    {
        public void BeforeInitialize(InitializationEngine engine)
        {
            CopyConfigurationFromWebConfigToAppConfig();
        }

        public void AfterInitialize(InitializationEngine engine)
        {
        }

        private void CopyConfigurationFromWebConfigToAppConfig()
        {
            CopyEpiserverFindConfigurationSection();
            CopyEPiServerFindCmsConfigurationSection();
        }

        private void CopyEpiserverFindConfigurationSection()
        {
            FileConfigurationSource instance = (FileConfigurationSource)ConfigurationSource.Instance;

            var episerverFindSection = (EPiServer.Find.Configuration)instance.ConfigurationInstance.GetSection("episerver.find");

            var currentAppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            currentAppConfig.AppSettings.Settings.Add("episerver:FindServiceUrl", episerverFindSection.ServiceUrl);
            currentAppConfig.AppSettings.Settings.Add("episerver:FindDefaultIndex", episerverFindSection.DefaultIndex);

            currentAppConfig.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("appSettings");
        }

        private void CopyEPiServerFindCmsConfigurationSection()
        {
            FileConfigurationSource instance = (FileConfigurationSource)ConfigurationSource.Instance;
            var currentAppConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

            var episerverFindCmsSection = new EPiServerFindCmsConfigurationSection(instance.ConfigurationInstance.GetSection("episerver.find.cms"));
            episerverFindCmsSection.DisableEventedIndexing = true;

#if NET48_OR_GREATER
            episerverFindCmsSection.DisableScheduledPageQueue = true;
#endif

            currentAppConfig.Sections.Remove("episerver.find.cms");
            currentAppConfig.Sections.Add("episerver.find.cms", episerverFindCmsSection);
            currentAppConfig.Save(ConfigurationSaveMode.Modified);

            ConfigurationManager.RefreshSection("episerver.find.cms");
        }

        public IEnumerable<IClearCommand> Reset()
        {
            return new List<IClearCommand>();
        }
    }
}
