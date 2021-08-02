using EPiServer.Templates.Alloy.Mvc;
using Lorem.Testing.EPiServer.CMS;
using Lorem.Testing.EPiServer.CMS.TestFrameworks;

namespace Alloy.Test.Services
{
    public class DefaultEpiserverEngine
        : EpiserverEngine<Startup>
    {
        public DefaultEpiserverEngine()
            : base(new CmsTestFrameworks())
        {
        }
    }
}
