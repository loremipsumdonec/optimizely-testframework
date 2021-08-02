using EPiServer.Validation;
using System.Collections.Generic;

namespace Lorem.Testing.EPiServer.CMS.Services
{
    public class ToggleContextValidationService
        : IContextValidationService
    {
        private readonly IValidationService _decorated;

        public ToggleContextValidationService(IValidationService decorated)
        {
            _decorated = decorated;
        }

        public bool Enabled { get; set; }

        public IEnumerable<ValidationError> Validate<T>(object instance, T context)
        {
            if(Enabled && _decorated is IContextValidationService contextValidationService)
            {
                return contextValidationService.Validate(instance, context);
            }

            return new List<ValidationError>();
        }

        public IEnumerable<ValidationError> Validate(object instance)
        {
            if (Enabled)
            {
                return _decorated.Validate(instance);
            }

            return new List<ValidationError>();
        }
    }
}
