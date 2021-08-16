using Lorem.Test.Framework.Optimizely.CMS.Modules;
using System;
using System.IO;

namespace Lorem.Test.Framework.Optimizely.CMS.Test.Services
{
    public class DefaultEngine
        : Engine
    {
        public DefaultEngine()
        {
            Add(
                new CmsTestModule(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Lorem\App_Data\blobs"))
                {
                    IamAwareThatTheDatabaseWillBeDeletedAndReCreated = true,
                    IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted = true
                }
            );

            Add(new SearchAndNavigationTestModule());
        }
    }
}
