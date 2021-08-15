using Lorem.Testing.Optimizely.CMS.Modules;
using System;
using System.IO;

namespace Lorem.Testing.Optimizely.CMS.Test.Services
{
    public class DefaultEngine
        : Engine
    {
        public DefaultEngine()
        {
            Add(new SearchAndNavigationTestModule());

            Add(
                new CmsTestModule(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..\..\..\Lorem\App_Data\blobs"))
                {
                    IamAwareThatTheDatabaseWillBeDeletedAndReCreated = true,
                    IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted = true
                }
            );
        }
    }
}
