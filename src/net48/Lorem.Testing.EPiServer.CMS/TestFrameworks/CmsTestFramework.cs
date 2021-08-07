using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using Lorem.Testing.EPiServer.CMS.Commands;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.TestFrameworks
{
    public class CmsTestFramework
        : IEpiserverTestFramework
    {
        private readonly string _connectionStringName;
        private string _connectionString;

        public CmsTestFramework()
            : this("EPiServerDB")
        {
        }

        public CmsTestFramework(string connectionStringName)
        {
            _connectionStringName = connectionStringName;
        }

        public void BeforeInitialize(InitializationEngine engine)
        {
            RemoveInitializationModules(engine);
            LoadConnectionString();
            CreateCmsDatabase();
        }

        private static void RemoveInitializationModules(InitializationEngine engine)
        {
            var modules = engine.Modules.ToList();

            modules.RemoveAll(m => m.GetType().FullName.StartsWith("EPiServer.Enterprise.Internal.DefaultSiteContentInitialization"));

            engine.Modules = modules;
        }

        public void AfterInitialize(InitializationEngine engine)
        {
            SaveStartupReport();
        }

        public IEnumerable<IClearCommand> Reset()
        {
            return new List<IClearCommand>()
            {
                new ClearContents(),
                new ClearCategories(),
                new ClearSites()
            };
        }

        private void CreateCmsDatabase()
        {
            string createDatabaseSqlStatement = GetCreateDatabaseSqlStatement();

            using (SqlConnection connection = new SqlConnection(GetConnectionStringToMaster()))
            {
                try
                {
                    connection.Open();

                    using (var command = new SqlCommand(createDatabaseSqlStatement, connection))
                    {
                        command.ExecuteNonQuery();
                    }

                }
                finally
                {
                    connection.Close();
                }
            }
        }

        private string GetDatabaseName()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);
            return builder.InitialCatalog;
        }

        private void LoadConnectionString()
        {
            ValidateConnectionString();

            var connectionStringsSection = ConfigurationSource.Instance.Get<ConnectionStringsSection>("connectionStrings");
            _connectionString = connectionStringsSection.ConnectionStrings[_connectionStringName].ConnectionString;
        }

        private void ValidateConnectionString()
        {
            var connectionStringsSection = ConfigurationSource.Instance.Get<ConnectionStringsSection>("connectionStrings");

            if (connectionStringsSection.ConnectionStrings[_connectionStringName] == null)
            {
                throw new ArgumentException($"Missing valid ConnectionString with name {_connectionStringName}");
            }
        }

        private string GetConnectionStringToMaster()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(_connectionString);
            builder.InitialCatalog = "master";

            return builder.ToString();
        }

        private string GetCreateDatabaseSqlStatement()
        {
            return string.Format(@"
                    IF DB_ID('{0}') is not null
	                    begin
		                    ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
		                    DROP DATABASE [{0}]
	                    end

                    CREATE DATABASE [{0}]", GetDatabaseName());
        }

        private void SaveStartupReport()
        {
            var meters = TimeMeters.GetAllRegistered()
                .OrderByDescending(t => t.Counters.Values.Max(sw => sw.ElapsedMilliseconds))
                .ToList();

            using (StreamWriter writer = new StreamWriter(@"C:\Temp\episerver_boot.csv"))
            {
                foreach (var meter in TimeMeters.GetAllRegistered())
                {
                    foreach (string key in meter.Counters.Keys)
                    {
                        var counter = meter.Counters[key];
                        writer.WriteLine($"{meter.Owner};{key};{counter.ElapsedMilliseconds}");
                    }
                }
            }
        }
    }
}
