using StructureMap;
using System;
namespace Lorem.Test.Framework.Optimizely.CMS.Services
{
    public sealed class NestedContext
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
