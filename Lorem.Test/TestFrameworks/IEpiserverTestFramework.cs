using EPiServer.Framework.Configuration;
using System;
using System.Configuration;
using System.Data.SqlClient;

namespace Lorem.Test.TestFrameworks
{
    public interface IEpiserverTestFramework
    {
        void BeforeInitialize();

        void AfterInitialize();

        void Reset();
    }
}
