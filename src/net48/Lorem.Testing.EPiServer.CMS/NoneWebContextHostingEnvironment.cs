using EPiServer.Web.Hosting;
using System.IO;
using System.Web.Hosting;

namespace Lorem.Testing.EPiServer.CMS
{
    internal class NoneWebContextHostingEnvironment
        : IHostingEnvironment
    {
        public NoneWebContextHostingEnvironment(string virtualPath, string physicalPath)
        {
            ApplicationPhysicalPath = physicalPath;
            ApplicationVirtualPath = virtualPath;
        }

        public string ApplicationID { get; set; }

        public string ApplicationPhysicalPath { get; set; }

        public string ApplicationVirtualPath { get; set; }

        public VirtualPathProvider VirtualPathProvider { get; private set; }

        public string MapPath(string virtualPath)
        {
            virtualPath = virtualPath.Trim(new char[] { ' ', '~', '/' }).Replace('/', '\\');
            return Path.Combine(ApplicationPhysicalPath, virtualPath);
        }

        public void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider)
        {
            VirtualPathProvider = virtualPathProvider;
        }
    }
}
