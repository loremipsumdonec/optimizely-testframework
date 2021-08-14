using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using Lorem.Testing.Optimizely.CMS.Commands;
using Lorem.Testing.Optimizely.CMS.Initialization;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS.TestFrameworks
{
    public class CmsTestFramework
        : ITestFramework
    {
        private readonly string _connectionStringName;
        private string _connectionString;
        private string _appDataPath;

        public CmsTestFramework()
            : this("EPiServerDB", null)
        {
        }

        public CmsTestFramework(string appDataPath)
            : this("EPiServerDB", appDataPath)
        {
        }

        public CmsTestFramework(string connectionStringName, string appDataPath)
        {
            _connectionStringName = connectionStringName;
            _appDataPath = appDataPath;

            ValidateAppDataPath();
        }

        public bool IamAwareThatTheDatabaseWillBeDeletedAndReCreated { get; set; }

        public bool IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted { get; set; }

        public void BeforeInitialize(InitializationEngine engine)
        {
            RemoveInitializationModules(engine);
            LoadConnectionString();
            CreateCmsDatabase();
        }

        private void RemoveInitializationModules(InitializationEngine engine)
        {
            var modules = engine.Modules.ToList();

            modules.RemoveAll(m => m.GetType().FullName.StartsWith("EPiServer.Enterprise.Internal.DefaultSiteContentInitialization"));

            var fileBlobProviderInitialization = (FileBlobProviderInitialization)modules.First(m=> m is FileBlobProviderInitialization);
            fileBlobProviderInitialization.AppDataPath = _appDataPath;

            engine.Modules = modules;
        }

        public void AfterInitialize(InitializationEngine engine)
        {
        }

        public IEnumerable<IClearCommand> Reset()
        {
            var clearContents = new ClearContents();

            if (!IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted)
            {
                throw new InvalidOperationException($"You need to be aware that the files and folders at the path {clearContents.GetPath()} will be deleted before each test.");
            }

            return new List<IClearCommand>()
            {
                clearContents,
                new ClearCategories(),
                new ClearSites()
            };
        }

        private void CreateCmsDatabase()
        {
            if (!IamAwareThatTheDatabaseWillBeDeletedAndReCreated)
            {
                throw new InvalidOperationException($"You need to be aware that the database specified with connectionString {_connectionStringName} will be deleted and re-created at the start of each test session. (\"{_connectionString}\")");
            }

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
    
        private void ValidateAppDataPath()
        {
            if(string.IsNullOrEmpty(_appDataPath))
            {
                return;
            }

            if(!_appDataPath.EndsWith("blobs"))
            {
                throw new ArgumentException($"The specified appDataPath ({_appDataPath}) does not appear to be a correct path to the Optimizely CMS blobs, as the path does not end with \"blobs\"");
            }
        }
    }
}
