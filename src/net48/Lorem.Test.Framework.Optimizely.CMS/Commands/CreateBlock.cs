using EPiServer;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;
using System.Globalization;

namespace Lorem.Test.Framework.Optimizely.CMS.Commands
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
                  ServiceLocator.Current.GetInstance<ContentAssetHelper>()
            )
        {
        }

        public CreateBlock(
            ContentType contentType,
            ContentReference parent,
            string name,
            IContentRepository repository,
            ContentAssetHelper helper)
        {
            ContentType = contentType;
            Parent = parent;
            Name = name;
            
            _repository = repository;
            _helper = helper;
        }

        public string Name { get; set; }

        public ContentType ContentType { get; set; }

        public bool HasAssetFolder { get; set; }

        public ContentReference Parent { get; set; }

        public CultureInfo Culture { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish | SaveAction.ForceCurrentVersion;

        public Action<object> Build { get; set; }

        public BlockData Execute()
        {
            var content = GetDefault(GetAssetFolder());
            content.Name = Name;

            if(Build != null)
            {
                Save(content, SaveAction.SkipValidation | SaveAction.Default);
                Build(content);
            }

            return Save(content);
        }

        private ContentReference GetAssetFolder()
        {
            var folder = _helper.GetOrCreateAssetFolder(Parent);

            if(folder == null)
            {
                return Parent;
            }

            return folder.ContentLink;
        }

        private IContent GetDefault(ContentReference parent)
        {
            return _repository.GetDefault<IContent>(
                    parent,
                    ContentType.ID,
                    Culture);
        }

        private BlockData Save(IContent content, SaveAction saveAction = SaveAction.Publish)
        {
            var contentReference = _repository.Save(
                content,
                saveAction,
                AccessLevel.NoAccess
            );

            var updated = (IReadOnly)_repository.Get<IContent>(contentReference);
            return (BlockData)updated.CreateWritableClone();
        }
    }
}
