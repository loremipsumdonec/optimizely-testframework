using EPiServer;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS.Commands
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

        public CultureInfo Culture { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish;

        public Action<object> Build { get; set; }

        public IContent Execute()
        {
            IContent content = Content;

            if(Culture != null && content is ILocalizable localizable)
            {
                if(!localizable.Language.Equals(Culture)
                    && localizable.ExistingLanguages.Any(c => c.Equals(Culture)))
                {
                    content = (IContent)_repository.Get<ContentData>(
                        content.ContentLink.ToReferenceWithoutVersion(),
                        Culture
                    ).CreateWritableClone();
                }
                else
                {
                    content = _repository.CreateLanguageBranch<IContent>(Content.ContentLink, Culture);
                    content.Name = Content.Name;
                }
            }

            Build?.Invoke(content);

            return (IContent)Save(content).CreateWritableClone();
        }

        private ContentData Save(IContent content)
        {
            var contentReference = _repository.Save(
                content,
                SaveAction,
                AccessLevel.NoAccess
            );

            return _repository.Get<ContentData>(contentReference);
        }
    }
}
