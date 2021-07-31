
using EPiServer.Framework.Initialization;

namespace Lorem.Testing.EPiServer.CMS.TestFrameworks
{
    public interface IEpiserverTestFramework
    {
        void BeforeInitialize(InitializationEngine engine);

        void AfterInitialize(InitializationEngine engine);

        void Reset();
    }
}
