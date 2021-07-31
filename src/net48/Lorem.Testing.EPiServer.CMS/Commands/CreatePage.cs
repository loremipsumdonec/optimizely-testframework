using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Validation;
using Lorem.Testing.EPiServer.CMS.Services;
using System;
using System.Globalization;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    internal class CreatePage
    {
        private readonly IContentRepository _repository;
        private readonly ToggleContextValidationService _service;

        public CreatePage(ContentType contentType, ContentReference parent, string name)
            : this(
                  contentType, 
                  parent, 
                  name, 
                  ServiceLocator.Current.GetInstance<IContentRepository>(),
                  ServiceLocator.Current.GetInstance<IValidationService>()
            )
        {
        }

        public CreatePage(
            ContentType contentType,
            ContentReference parent,
            string name,
            IContentRepository repository,
            IValidationService service)
        {
            ContentType = contentType;
            Parent = parent;
            Name = name;
            
            _repository = repository;
            _service = (ToggleContextValidationService)service;
        }

        public string Name { get; set; }

        public ContentType ContentType { get; set; }

        public ContentReference Parent { get; set; }

        public CultureInfo Language { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish | SaveAction.ForceCurrentVersion;

        public Action<object> Build { get; set; }

        public PageData Execute()
        {
            var content = GetDefault();
            content.Name = Name;

            if(Build != null)
            {
                _service.Enabled = false;
                Save(content);
                _service.Enabled = true;

                Build(content);
            }

            return Save(content);
        }

        private IContent GetDefault()
        {
            return _repository.GetDefault<IContent>(
                    Parent,
                    ContentType.ID,
                    Language);
        }

        private PageData Save(IContent content)
        {
            var contentReference = _repository.Save(
                content,
                SaveAction,
                AccessLevel.NoAccess
            );

            return _repository.Get<PageData>(contentReference).CreateWritableClone();
        }
    }
}
