using Lorem.Testing.Optimizely.CMS.TestFrameworks;
using System;
using System.IO;

namespace Lorem.Testing.Optimizely.CMS.Test.Services
{
    public class DefaultEpiserverEngine
        : EpiserverEngine
    {
        public DefaultEpiserverEngine()
        {
            Add(
                new CmsTestFramework(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Lorem\App_Data\blobs"))
            );
        }
    }
}
