using EPiServer;
using EPiServer.ServiceLocation;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Lorem.Test.Services
{
    public class EpiserverEngineFirstIteration<TEntryPoint>
        : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private bool _started;

        public void Start()
        {
            if(_started)
            {
                return;
            }

            CreateCmsDatabase();

            var repository = Services.GetInstance(typeof(IContentRepository));

            if (repository == null)
            {
                throw new InvalidOperationException("Failed starting");
            }

            _started = true;
        }

        public void Stop()
        {
            _started = false;
        }

        private void CreateCmsDatabase()
        {
            var configuration = GetConfiguration();
            CreateCmsDatabase(configuration.GetConnectionString("EPiServerDB"));
        }

        private IConfiguration GetConfiguration()
        {
            return new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.Development.json", optional: true)
                .AddEnvironmentVariables()
                .Build();
        }

        private void CreateCmsDatabase(string connectionString)
        {
            string createDatabaseSqlStatement = GetCreateDatabaseSqlStatement(connectionString);

            using (SqlConnection connection = new SqlConnection(GetConnectionStringToMaster(connectionString)))
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

        private string GetConnectionStringToMaster(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            builder.InitialCatalog = "master";

            return builder.ToString();
        }

        private string GetDatabaseName(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }

        private string GetCreateDatabaseSqlStatement(string connectionString)
        {
            return string.Format(@"
                    IF DB_ID('{0}') is not null
	                    begin
		                    ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
		                    DROP DATABASE [{0}]
	                    end

                    CREATE DATABASE [{0}]", GetDatabaseName(connectionString));
        }
    }
}
