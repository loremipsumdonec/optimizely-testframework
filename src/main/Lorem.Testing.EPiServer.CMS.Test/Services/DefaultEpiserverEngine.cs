using Lorem.Testing.EPiServer.CMS.TestFrameworks;

namespace Lorem.Testing.EPiServer.CMS.Test.Services
{
    public class DefaultEpiserverEngine
        : EpiserverEngine<Startup>
    {
        public DefaultEpiserverEngine()
            : base(new CmsTestFramework())
        {
        }
    }
}
