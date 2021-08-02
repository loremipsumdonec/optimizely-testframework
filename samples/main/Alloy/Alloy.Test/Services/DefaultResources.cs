using Lorem.Testing.EPiServer.CMS.Utility;
using System;

namespace Alloy.Test.Services
{
    public class DefaultResources
        : Resources
    {
        public DefaultResources()
            : base(System.IO.Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Resources"))
        {
        }
    }
}
