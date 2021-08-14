using Lorem.Testing.Optimizely.CMS.Test.Services;
using System.IO;
using Xunit;

namespace Lorem.Testing.Optimizely.CMS.Test.Invalid
{
    [Trait("verification", "required")]
    public class MissingConfigurationTests
    {
        [Fact]
        public void MissingWebConfig_ShouldThrowFileNotFoundException()
        {
            var engine = new DefaultEngine();

            Assert.Throws<FileNotFoundException>(
                () => engine.Start()
            );
        }
    }
}
