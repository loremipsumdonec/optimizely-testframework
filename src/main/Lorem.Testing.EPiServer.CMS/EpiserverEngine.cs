using Microsoft.AspNetCore.Mvc.Testing;
using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.IO;
using System.Collections.Generic;
using Lorem.Testing.EPiServer.CMS.TestFrameworks;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using System.Linq;
using System;
using Lorem.Testing.EPiServer.CMS.Commands;
using EPiServer;

namespace Lorem.Testing.EPiServer.CMS
{
    public class EpiserverEngine<TEntryPoint>
        : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
    {
        private readonly List<IEpiserverTestFramework> _frameworks = new();
        private List<ContentType> _contentTypes;
        private bool _started;

        public EpiserverEngine()
        {
        }

        public EpiserverEngine(params IEpiserverTestFramework[] frameworks)
        {
            _frameworks = new List<IEpiserverTestFramework>(frameworks);
        }

        public void Add(IEpiserverTestFramework testFramework)
        {
            _frameworks.Add(testFramework);
        }

        public void Start()
        {
            if(_started)
            {
                Reset();
                return;
            }

            BeforeInitialize();

            var repository = Services.GetInstance(typeof(IContentRepository));

            if(repository == null)
            {
                throw new InvalidOperationException("Failed starting");
            }

            Reset();

            AfterInitialize();

            _started = true;
        }

        public void Stop() 
        {
            _started = false;
        }

        public ContentType GetContentType(Type type)
        {
            if (_contentTypes == null)
            {
                var repository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                _contentTypes = new List<ContentType>(repository.List());
            }

            var contentType = _contentTypes.FirstOrDefault(c => c.ModelType != null && c.ModelType.Equals(type));

            if (contentType == null)
            {
                throw new ArgumentException($"could not find a valid content type for type {type}");
            }

            return contentType;
        }

        private void Reset()
        {
            List<IClearCommand> clearCommands = new List<IClearCommand>();

            foreach (var testFramework in _frameworks)
            {
                clearCommands.AddRange(
                    testFramework.Reset()
                );
            }

            foreach(var command in clearCommands)
            {
                command.Clear();
            }
        }

        private void BeforeInitialize()
        {
            var configuration = GetConfiguration();

            foreach (var testFramework in _frameworks)
            {
                testFramework.BeforeInitialize(configuration);
            }
        }

        private void AfterInitialize()
        {
            foreach (var testFramework in _frameworks)
            {
                testFramework.AfterInitialize();
            }
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
            //string connectionString = configuration.GetConnectionString("EPiServerDB");

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
