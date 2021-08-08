using Lorem.Testing.Optimizely.CMS.Utility;
using System;

namespace Lorem.Testing.Optimizely.CMS.Test.Services
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
