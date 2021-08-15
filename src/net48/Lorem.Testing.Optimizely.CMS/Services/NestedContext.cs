using EPiServer.ServiceLocation;
using EPiServer.ServiceLocation.AutoDiscovery;
using StructureMap;
using System;
using System.Collections.Generic;
namespace Lorem.Testing.Optimizely.CMS.Services
{

    public class NestedContext
        : IDisposable
    {
        private readonly ServiceLocatorDecorator _locator;

        public NestedContext(ServiceLocatorDecorator locator)
        {
            _locator = locator;
        }

        public IContainer Container => _locator.Current;
        
        public void Dispose()
        {
            _locator.Pop();
        }
    }
}
