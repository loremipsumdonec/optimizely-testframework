using EPiServer.Framework.Initialization;
using Lorem.Testing.Optimizely.CMS.Commands;
using System.Collections.Generic;

namespace Lorem.Testing.Optimizely.CMS.Modules
{
    public interface ITestModule
    {
        void BeforeInitialize(InitializationEngine engine);

        void AfterInitialize(InitializationEngine engine);

        IEnumerable<IClearCommand> Reset();
    }
}
