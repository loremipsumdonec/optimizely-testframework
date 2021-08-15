using EPiServer.ServiceLocation;
using EPiServer.ServiceLocation.AutoDiscovery;
using StructureMap;
using System;
using System.Collections.Generic;
namespace Lorem.Testing.Optimizely.CMS.Services
{

    public class ServiceLocatorDecorator
        : IServiceLocator
    {
        private readonly IServiceLocator _decorated;

        public ServiceLocatorDecorator(IServiceLocator decorated)
        {
            _decorated = decorated;
        }

        public IContainer Current { get; private set; }

        public NestedContext Push()
        {
            var container = _decorated.GetInstance<IContainer>();
            Current = container.GetNestedContainer();

            return new NestedContext(this);
        }

        public void Pop()
        {
            Current.Dispose();
            Current = null;
        }

        public IEnumerable<object> GetAllInstances(Type serviceType)
        {
            if (Current != null)
            {
                List<object> instances = new List<object>();

                foreach(object instance in Current.GetAllInstances(serviceType))
                {
                    instances.Add(instance);
                }

                return instances;
            }

            return _decorated.GetAllInstances(serviceType);
        }

        public object GetInstance(Type serviceType)
        {
            if(Current != null)
            {
                return Current.GetInstance(serviceType);
            }

            return _decorated.GetInstance(serviceType);
        }

        public TService GetInstance<TService>()
        {
            if (Current != null)
            {
                return Current.GetInstance<TService>();
            }

            return _decorated.GetInstance<TService>();
        }

        public object GetService(Type serviceType)
        {
            if (Current != null)
            {
                return Current.GetInstance(serviceType);
            }

            return _decorated.GetService(serviceType);
        }

        public bool TryGetExistingInstance(Type serviceType, out object instance)
        {
            return _decorated.TryGetExistingInstance(serviceType, out instance);
        }
    }
}
