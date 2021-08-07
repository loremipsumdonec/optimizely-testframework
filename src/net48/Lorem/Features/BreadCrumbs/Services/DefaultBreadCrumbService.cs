using EPiServer;
using EPiServer.Core;
using EPiServer.ServiceLocation;
using EPiServer.Web.Routing;
using Lorem.Features.BreadCrumbs.Models;
using Lorem.Models.Pages;
using System.Collections.Generic;

namespace Lorem.Features.BreadCrumbs.Services
{
    [ServiceConfiguration(ServiceType = typeof(IBreadCrumbService))]
    public class DefaultBreadCrumbService
        : IBreadCrumbService
    {
        private readonly IContentLoader _loader;
        private readonly IUrlResolver _resolver;

        public DefaultBreadCrumbService(IContentLoader loader, IUrlResolver resolver)
        {
            _loader = loader;
            _resolver = resolver;
        }

        public List<BreadCrumb> GetBreadCrumbs(PageData page)
        {
            List<BreadCrumb> breadcrumbs = new List<BreadCrumb>();
            Stack<ContentReference> stack = new Stack<ContentReference>();
            stack.Push(page.ContentLink);

            while (stack.Count > 0)
            {
                var reference = stack.Pop();
                var current = _loader.Get<PageData>(reference, page.Language);

                if (current == null)
                {
                    continue;
                }

                var breadcrumb = CreateBreadCrumb(current);

                if (breadcrumb != null)
                {
                    breadcrumbs.Add(breadcrumb);
                }

                if (!ContentReference.IsNullOrEmpty(current.ParentLink))
                {
                    stack.Push(current.ParentLink);
                }
            }

            breadcrumbs.Reverse();

            return breadcrumbs;
        }

        private BreadCrumb CreateBreadCrumb(PageData page)
        {
            if(page is SitePage sitePage && sitePage.VisibleInBreadCrumb)
            {
                return new BreadCrumb()
                {
                    Text = sitePage.Heading
                };
            }

            return null;
        }
    }
}