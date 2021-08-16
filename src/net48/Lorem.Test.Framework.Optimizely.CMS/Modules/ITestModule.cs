using EPiServer.Framework.Initialization;
using Lorem.Test.Framework.Optimizely.CMS.Commands;
using System.Collections.Generic;

namespace Lorem.Test.Framework.Optimizely.CMS.Modules
{
    public interface ITestModule
    {
        void BeforeInitialize(InitializationEngine engine);

        void AfterInitialize(InitializationEngine engine);

        IEnumerable<IClearCommand> Reset();
    }
}
