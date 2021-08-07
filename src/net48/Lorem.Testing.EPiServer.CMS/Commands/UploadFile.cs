using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using System;
using System.Globalization;
using System.IO;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    public class UploadFile
    {
        private readonly ContentAssetHelper _helper;
        private readonly IContentRepository _repository;
        private readonly IBlobFactory _blobFactory;

        public UploadFile(
            string name,
            string file,
            ContentType contentType,
            ContentReference parent,
            CultureInfo language = null
        ): this(
            name,
            file,
            contentType,
            parent,
            language,
            ServiceLocator.Current.GetInstance<ContentAssetHelper>(),
            ServiceLocator.Current.GetInstance<IContentRepository>(),
            ServiceLocator.Current.GetInstance<IBlobFactory>()
        )
        {
        }

        public UploadFile(
            string name,
            string file,
            ContentType contentType,
            ContentReference parent,
            CultureInfo language,
            ContentAssetHelper helper,
            IContentRepository repository,
            IBlobFactory blobFactory
            )
        {
            Name = name;
            File = file;
            ContentType = contentType;
            Parent = parent;
            Language = language;

            _helper = helper;
            _repository = repository;
            _blobFactory = blobFactory;

            Validate();
        }

        public string Name { get; set; }

        public string File { get; set; }

        public ContentType ContentType { get; set; }

        public bool HasAssetFolder { get; set; }

        public ContentReference Parent { get; set; }

        public CultureInfo Language { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish | SaveAction.ForceCurrentVersion;

        public Action<object> Build { get; set; }

        public MediaData Execute() 
        {
            var mediaData = _repository.GetDefault<MediaData>(
                GetAssetFolder(),
                ContentType.ID,
                Language
            );

            Upload(mediaData);

            if (Build != null)
            {
                Save(mediaData);
                Build.Invoke(mediaData);
            }

            return Save(mediaData);
        }

        private void Validate()
        {
            /*
            var options = ServiceLocator.Current.GetInstance<BlobOptions>();

            if (options.DefaultProvider != "fileBlobProvider")
            {
                throw new InvalidOperationException(
                    "There must be a registered FileBlobProvider with name fileBlobProvider to be able to handle MediaData. Create an IConfigurableModule in the test project and add a FileBlobProvider with context.Services.AddFileBlobProvider(\"fileBlobProvider\", \"path where you want to save the files\")"
                );
            }
            */
        }

        private ContentReference GetAssetFolder()
        {
            if (HasAssetFolder)
            {
                return Parent;
            }

            return _helper.GetOrCreateAssetFolder(Parent).ContentLink;
        }

        private void Upload(MediaData mediaData)
        {
            var extension = Path.GetExtension(File);

            using (var stream = System.IO.File.OpenRead(File))
            {
                var blob = _blobFactory.CreateBlob(mediaData.BinaryDataContainer, extension);
                blob.Write(stream);

                mediaData.Name = Name;
                mediaData.StartPublish = DateTime.Now;
                mediaData.BinaryData = blob;
            }
        }

        private MediaData Save(IContent page)
        {
            var contentReference = _repository.Save(
                page,
                SaveAction,
                AccessLevel.NoAccess
            );

            return (MediaData)_repository.Get<MediaData>(contentReference).CreateWritableClone();
        }
    }
}
