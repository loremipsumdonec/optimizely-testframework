﻿using EPiServer.Data;
using EPiServer.Framework;
using EPiServer.Framework.Blobs;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
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
            context.Services.AddSingleton<FileBlobProviderOptions>((_) =>
            {
                var options = new FileBlobProviderOptions();
                options.Path = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Lorem\App_Data\blobs");
                return options;
            });

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
