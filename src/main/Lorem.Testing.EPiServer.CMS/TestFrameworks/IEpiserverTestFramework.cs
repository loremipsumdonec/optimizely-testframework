using Lorem.Testing.EPiServer.CMS.Commands;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Lorem.Testing.EPiServer.CMS.TestFrameworks
{
    public interface IEpiserverTestFramework
    {
        void BeforeInitialize(IConfiguration configuration);

        void AfterInitialize();

        IEnumerable<IClearCommand> Reset();
    }
}
