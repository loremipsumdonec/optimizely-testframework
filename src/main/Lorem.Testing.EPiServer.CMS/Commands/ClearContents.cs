using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Blobs;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    public class ClearContents
        : IClearCommand
    {
        private readonly IContentRepository _repository;
        private readonly FileBlobProvider _fileBlobProvider;

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
                ServiceLocator.Current.GetInstance<IBlobProviderRegistry>()
            )
        {
        }

        public ClearContents(
            ContentReference root,
            List<string> isASysType,
            IContentRepository repository,
            IBlobProviderRegistry registry)
        {
            _repository = repository;
            _fileBlobProvider = (FileBlobProvider)registry.GetProvider(new Uri("//default"));

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
            if(Directory.Exists(_fileBlobProvider.Path))
            {
                Directory.Delete(_fileBlobProvider.Path, true);
            }
            
            Directory.CreateDirectory(_fileBlobProvider.Path);
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
