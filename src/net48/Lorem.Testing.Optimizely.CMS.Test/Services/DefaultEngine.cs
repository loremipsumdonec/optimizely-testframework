using Lorem.Testing.Optimizely.CMS.TestFrameworks;
using System;
using System.IO;

namespace Lorem.Testing.Optimizely.CMS.Test.Services
{
    public class DefaultEngine
        : Engine
    {
        public DefaultEngine()
        {
            Add(
                new CmsTestFramework(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Lorem\App_Data\blobs"))
            );
        }
    }
}
