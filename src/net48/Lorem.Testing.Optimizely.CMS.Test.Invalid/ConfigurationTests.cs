using Lorem.Testing.Optimizely.CMS.Test.Services;
using Lorem.Testing.Optimizely.CMS.Modules;
using System;
using System.IO;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test.Invalid
{
    [Trait("verification", "required")]
    public class ConfigurationTests
    {
        [Fact]
        public void MissingWebConfig_ShouldThrowFileNotFoundException()
        {
            var engine = new DefaultEngine();

            Assert.Throws<FileNotFoundException>(
                () => engine.Start()
            );
        }

        [Theory]
        [InlineData("/invalid/path")]
        [InlineData("/invalid/blobs/")]
        public void CreateCmsModule_WithAppDataPathNotEndingWithBlobs_ThrowsArgumentException(string invalidPath)
        {
            Assert.Throws<ArgumentException>(
                () => new CmsTestModule(invalidPath)
            );
        }

        [Theory]
        [InlineData("/valid/blobs")]
        public void CreateCmsModule_WithAppDataPathEndingWithBlobs_Success(string validPath)
        {
            var _ = new CmsTestModule(validPath);
        }
    }
}
