using EPiServer;
using EPiServer.Core;
using EPiServer.Data.Entity;
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
    internal class CreateBlock
    {
        private readonly IContentRepository _repository;
        private readonly ContentAssetHelper _helper;

        public CreateBlock(ContentType contentType, ContentReference parent, string name)
            : this(
                  contentType, 
                  parent, 
                  name, 
                  ServiceLocator.Current.GetInstance<IContentRepository>(),
                  ServiceLocator.Current.GetInstance<IValidationService>(),
                  ServiceLocator.Current.GetInstance<ContentAssetHelper>()
            )
        {
        }

        public CreateBlock(
            ContentType contentType,
            ContentReference parent,
            string name,
            IContentRepository repository,
            IValidationService service,
            ContentAssetHelper helper)
        {
            ContentType = contentType;
            Parent = parent;
            Name = name;
            
            _repository = repository;
            //_service = (ToggleContextValidationService)service;
            _helper = helper;
        }

        public string Name { get; set; }

        public ContentType ContentType { get; set; }

        public bool HasAssetFolder { get; set; }

        public ContentReference Parent { get; set; }

        public CultureInfo Language { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish | SaveAction.ForceCurrentVersion;

        public Action<object> Build { get; set; }

        public BlockData Execute()
        {
            var content = GetDefault(GetAssetFolder());
            content.Name = Name;

            if(Build != null)
            {
                //_service.Enabled = false;
                Save(content);
                //_service.Enabled = true;

                Build(content);
            }

            return Save(content);
        }

        private ContentReference GetAssetFolder()
        {
            if (HasAssetFolder)
            {
                return Parent;
            }

            return _helper.GetOrCreateAssetFolder(Parent).ContentLink;
        }

        private IContent GetDefault(ContentReference parent)
        {
            return _repository.GetDefault<IContent>(
                    parent,
                    ContentType.ID,
                    Language);
        }

        private BlockData Save(IContent content)
        {
            var contentReference = _repository.Save(
                content,
                SaveAction,
                AccessLevel.NoAccess
            );

            var updated = (IReadOnly)_repository.Get<IContent>(contentReference);
            return (BlockData)updated.CreateWritableClone();
        }
    }
}
