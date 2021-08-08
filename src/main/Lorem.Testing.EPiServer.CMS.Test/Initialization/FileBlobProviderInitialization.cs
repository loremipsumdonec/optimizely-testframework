using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;

namespace Alloy.Test.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(DataInitialization))]
    public class FileBlobProviderInitialization
        : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            context.Services.AddFileBlobProvider(
                "fileBlobProvider",
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Lorem\App_Data\blobs")
            );
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
