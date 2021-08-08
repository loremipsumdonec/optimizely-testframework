using EPiServer;
using EPiServer.Core;
using EPiServer.Security;
using EPiServer.ServiceLocation;

namespace Lorem.Testing.Optimizely.CMS.Commands
{
    public class MoveContent
    {
        private readonly IContentRepository _repository;

        public MoveContent(IContent content, ContentReference destination)
            : this(content, destination, ServiceLocator.Current.GetInstance<IContentRepository>())
        {
        }

        public MoveContent(
            IContent content,
            ContentReference destination, 
            IContentRepository repository)
        {
            Content = content;
            Destination = destination;
            _repository = repository;
        }

        public IContent Content { get; set; }

        public ContentReference Destination { get; set; }

        public IContent Execute()
        {
            _repository.Move(
                Content.ContentLink,
                Destination,
                AccessLevel.NoAccess,
                AccessLevel.NoAccess
            );

            return (IContent)_repository.Get<ContentData>(Content.ContentLink).CreateWritableClone();
        }
    }
}
