using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System.IO;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    public class DeleteContent
    {
        private readonly IContentRepository _repository;

        public DeleteContent(IContent content,bool forceDelete)
            : this(content, forceDelete, ServiceLocator.Current.GetInstance<IContentRepository>())
        {
        }

        public DeleteContent(
            IContent content, 
            bool forceDelete, 
            IContentRepository repository)
        {
            Content = content;
            ForceDelete = forceDelete;
            _repository = repository;
        }

        public IContent Content { get; set; }

        public bool ForceDelete { get; set; }

        public void Execute()
        {
            if (ForceDelete)
            {
                if (Content is MediaData mediaData)
                {
                    Delete(mediaData);
                }

                _repository.Delete(
                    Content.ContentLink,
                    true
                );

                return;
            }

            _repository.Move(
                Content.ContentLink,
                ContentReference.WasteBasket,
                AccessLevel.NoAccess,
                AccessLevel.NoAccess
            );
        }

        private void Delete(MediaData mediaData)
        {
            FileBlob blob = (FileBlob)mediaData.BinaryData;
            string path = Path.GetDirectoryName(blob.FilePath);

            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
        }
    }
}
