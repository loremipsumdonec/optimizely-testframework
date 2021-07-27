using EPiServer;
using EPiServer.Core;
using EPiServer.Framework.Configuration;
using EPiServer.Security;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;

namespace Lorem.Test.TestFrameworks
{
    public class CmsTestFrameworks
        : IEpiserverTestFramework
    {
        private readonly string _connectionStringName;
        private string _connectionString;

        public CmsTestFrameworks()
            : this("EPiServerDB")
        {
        }

        public CmsTestFrameworks(string connectionStringName)
        {
            _connectionStringName = connectionStringName;

        }

        public void BeforeInitialize()
        {
            LoadConnectionString();
            CreateCmsDatabase();
        }

        public void AfterInitialize()
        {
        }

        public void Reset()
        {
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
    }

    public class ClearContents
    {
        private readonly IContentRepository _repository;
        private readonly ContentReference _root;
        private readonly CultureInfo _culture;
        private readonly List<string> _ignoreTypes;

        public ClearContents(IContentRepository repository)
        {
            _repository = repository;

            _ignoreTypes = new List<string>() {
                "Root",
                "SysContentAssets",
                "SysGlobalAssets",
                "SysRecycleBin"
            };
        }

        public void Execute()
        {
            foreach (var content in _repository.GetChildren<IContent>(_root))
            {
                if (!_ignoreTypes.Contains(content.Name))
                {
                    if (content is BlockData
                        || content is ContentAssetFolder
                        || content is MediaData
                        || content is ContentFolder)
                    {
                    }
                }
            }
        }

        private void Delete(IContent content)
        {
            _repository.Delete(content.ContentLink,true,AccessLevel.NoAccess);
        }
    }
}
