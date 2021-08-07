using EPiServer.Framework.Initialization;
using Lorem.Testing.EPiServer.CMS.Commands;
using System.Collections.Generic;

namespace Lorem.Testing.EPiServer.CMS.TestFrameworks
{
    public interface IEpiserverTestFramework
    {
        void BeforeInitialize(InitializationEngine engine);

        void AfterInitialize(InitializationEngine engine);

        IEnumerable<IClearCommand> Reset();
    }
}
