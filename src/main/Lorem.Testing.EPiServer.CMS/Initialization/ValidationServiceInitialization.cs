using EPiServer.Framework;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Validation;
using EPiServer.Validation.Internal;
using Lorem.Testing.EPiServer.CMS.Services;

namespace Lorem.Testing.EPiServer.CMS.Initialization
{
    [InitializableModule]
    [ModuleDependency(typeof(ValidationService))]
    public class ValidationServiceInitialization
        : IConfigurableModule
    {
        public void ConfigureContainer(ServiceConfigurationContext context)
        {
            //context.Services.Intercept<IValidationService>((_, service) => new ToggleContextValidationService(service));
        }

        public void Initialize(InitializationEngine context)
        {
        }

        public void Uninitialize(InitializationEngine context)
        {
        }
    }
}
