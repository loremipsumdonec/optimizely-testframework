using System.Configuration;

namespace Lorem.Testing.Optimizely.SearchAndNavigation.Configuration
{
    public class EPiServerFindCmsConfigurationSection
        : EPiServer.Find.Cms.Configuration
    {
        public EPiServerFindCmsConfigurationSection()
        {
        }

        public EPiServerFindCmsConfigurationSection(ConfigurationSection source)
        {
            Properties.Add(new ConfigurationProperty("disableEventedIndexing", typeof(bool)));
            Properties.Add(new ConfigurationProperty("disableScheduledPageQueue", typeof(bool)));
            Properties.Add(new ConfigurationProperty("deleteLanguageRoutingDuplicatesOnIndex", typeof(bool)));

            if (source != null)
            {
                Reset(source);
            }
        }

        public new bool DisableEventedIndexing
        {
            get
            {
                return base.DisableEventedIndexing;
            }
            set
            {
                SetPropertyValue(Properties["disableEventedIndexing"], value, true);
            }
        }

        public new bool DisableScheduledPageQueue
        {
            get
            {
                return base.DisableScheduledPageQueue;
            }
            set
            {
                SetPropertyValue(Properties["disableScheduledPageQueue"], value, true);
            }
        }

        public new bool DeleteLanguageRoutingDuplicatesOnIndex
        {
            get
            {
                return base.DeleteLanguageRoutingDuplicatesOnIndex;
            }
            set
            {
                SetPropertyValue(Properties["deleteLanguageRoutingDuplicatesOnIndex"], value, true);
            }
        }
    }
}
