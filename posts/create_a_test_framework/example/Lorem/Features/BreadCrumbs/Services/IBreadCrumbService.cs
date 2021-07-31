using EPiServer.Core;
using Lorem.Features.BreadCrumbs.Models;
using System.Collections.Generic;

namespace Lorem.Features.BreadCrumbs.Services
{
    public interface IBreadCrumbService
    {
        List<BreadCrumb> GetBreadCrumbs(PageData page);
    }
}