using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    public class UpdateContent
    {
        private readonly IContentRepository _repository;

        public UpdateContent(IContent content)
            : this(
                content,
                ServiceLocator.Current.GetInstance<IContentRepository>()
            )
        {
        }

        public UpdateContent(
            IContent content,
            IContentRepository repository)
        {
            Content = content;
            _repository = repository;
        }

        public IContent Content { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish;

        public Action<object> Build { get; set; }

        public IContent Execute()
        {
            Build?.Invoke(Content);
            var content = Save();

            return (IContent)content.CreateWritableClone();
        }

        private ContentData Save()
        {
            var contentReference = _repository.Save(
                Content,
                SaveAction,
                AccessLevel.NoAccess
            );

            return _repository.Get<ContentData>(contentReference);
        }
    }
}
