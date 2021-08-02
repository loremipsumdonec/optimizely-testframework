using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System.Collections.Generic;
using System.IO;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    public class ClearContents
        : IClearCommand
    {
        private readonly IContentRepository _repository;
        private readonly FileBlobProviderOptions _fileBlobProviderOptions;

        public ClearContents()
            : this(ContentReference.RootPage)
        {
        }

        public ClearContents(ContentReference root)
            : this(
                root,
                new List<string>() {
                    "SysContentAssets",
                    "SysGlobalAssets",
                    "Recycle-Bin"
                },
                ServiceLocator.Current.GetInstance<IContentRepository>(),
                ServiceLocator.Current.GetInstance<FileBlobProviderOptions>()
            )
        {
        }

        public ClearContents(
            ContentReference root,
            List<string> isASysType,
            IContentRepository repository,
            FileBlobProviderOptions fileBlobProviderOptions)
        {
            _repository = repository;
            _fileBlobProviderOptions = fileBlobProviderOptions;

            Root = root;
            IsASysType = isASysType;
        }

        public ContentReference Root { get; set; }

        public List<string> IsASysType { get; set; }

        public void Clear()
        {
            ClearContentsInDatabase();
            ClearFiles();
        }

        private void ClearContentsInDatabase()
        {
            foreach (var child in _repository.GetChildren<IContent>(Root))
            {
                if (child is IRoutable routable && IsASysType.Contains(routable.RouteSegment))
                {
                    foreach (var content in _repository.GetChildren<IContent>(child.ContentLink))
                    {
                        Delete(content);
                    }

                    continue;
                }

                Delete(child);
            }
        }

        private void ClearFiles() 
        {
            if(Directory.Exists(_fileBlobProviderOptions.Path))
            {
                Directory.Delete(_fileBlobProviderOptions.Path, true);
            }
            
            Directory.CreateDirectory(_fileBlobProviderOptions.Path);
        }

        private void Delete(IContent content)
        {
            _repository.Delete
                   (content.ContentLink,
                   true,
                   AccessLevel.NoAccess
               );
        }
    }
}
