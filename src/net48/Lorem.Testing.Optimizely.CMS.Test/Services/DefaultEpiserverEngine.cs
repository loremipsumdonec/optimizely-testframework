using Lorem.Testing.Optimizely.CMS.TestFrameworks;

namespace Lorem.Testing.Optimizely.CMS.Test.Services
{
    public class DefaultEpiserverEngine
        : EpiserverEngine
    {
        public DefaultEpiserverEngine()
            : base(new CmsTestFramework())
        {
        }
    }
}
