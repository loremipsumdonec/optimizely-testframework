using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;

namespace Lorem.Test.Framework.Optimizely.CMS.Initialization
{
    [ModuleDependency(typeof(DataInitialization))]
    public class FileBlobProviderInitialization
        : IConfigurableModule
    {
        public string AppDataPath { get; set; }

        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            if(!string.IsNullOrEmpty(AppDataPath))
            {
                context.Services.AddFileBlobProvider(
                    "fileBlobProvider",
                    AppDataPath
                );

                return;
            }

            context.Services.AddBlobProvider<FileBlobProvider>("fileBlobProvider");
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
