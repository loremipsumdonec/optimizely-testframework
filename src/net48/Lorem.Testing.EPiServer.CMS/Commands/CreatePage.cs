using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;
using System.Globalization;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    internal class CreatePage
    {
        private readonly IContentRepository _repository;

        public CreatePage(ContentType contentType, ContentReference parent, string name)
            : this(
                  contentType, 
                  parent, 
                  name, 
                  ServiceLocator.Current.GetInstance<IContentRepository>()
            )
        {
        }

        public CreatePage(
            ContentType contentType,
            ContentReference parent,
            string name,
            IContentRepository repository)
        {
            ContentType = contentType;
            Parent = parent;
            Name = name;
            
            _repository = repository;
        }

        public string Name { get; set; }

        public ContentType ContentType { get; set; }

        public ContentReference Parent { get; set; }

        public CultureInfo Culture { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish | SaveAction.ForceCurrentVersion;

        public Action<object> Build { get; set; }

        public PageData Execute()
        {
            var content = GetDefault();
            content.Name = Name;

            if(Build != null)
            {
                Save(content, SaveAction.SkipValidation | SaveAction.Default);
                Build(content);
            }

            return Save(content);
        }

        private IContent GetDefault()
        {
            return _repository.GetDefault<IContent>(
                    Parent,
                    ContentType.ID,
                    Culture);
        }

        private PageData Save(IContent content, SaveAction saveAction = SaveAction.Publish)
        {
            var contentReference = _repository.Save(
                content,
                saveAction,
                AccessLevel.NoAccess
            );

            return _repository.Get<PageData>(contentReference).CreateWritableClone();
        }
    }
}
