using EPiServer;
using EPiServer.Core;
using EPiServer.Framework;
using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Web.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Lorem.Test.Services
{
    public class EpiserverEngineThirdIteration
    {
        private static EpiserverEngineThirdIteration _singelton;
        private InitializationEngine _engine;

        public static EpiserverEngineThirdIteration GetInstance()
        {
            if(_singelton == null)
            {
                _singelton = new EpiserverEngineThirdIteration();
            }

            return _singelton;
        }

        private EpiserverEngineThirdIteration()
        {
        }

        public void Start()
        {
            if(_engine != null)
            {
                Reset();
                return;
            }

            string webConfig = GetWebConfig();
            LoadConfigurationSource(webConfig);
            LoadHostingEnvironment(webConfig);
            CreateDatabase();

            List<IInitializableModule> modules = null;
            var assemblies = new AssemblyList(true).AllowedAssemblies;

            if(_engine == null)
            {
                _engine = new InitializationEngine(modules, HostType.TestFramework, assemblies);
                _engine.Initialize();
            }
        }

        private void Reset()
        {
            ClearContents();
        }

        private void ClearContents()
        {
            var repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var ignoreNames = new List<string>() {
                "Root",
                "Recycle Bin",
                "SysGlobalAssets",
                "SysContentAssets"
            };

            foreach (var content in repository.GetChildren<IContent>(ContentReference.RootPage))
            {
                if(ignoreNames.Contains(content.Name))
                {
                    continue;
                }

                repository.Delete(content.ContentLink, true, AccessLevel.NoAccess);
            }
        }

        public void Stop()
        {
            _engine.Uninitialize();
        }

        private string GetWebConfig()
        {
            string webConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");

            if (!File.Exists(webConfig))
            {
                throw new FileNotFoundException($"Could not load Web.config at expected path {webConfig}");
            }

            return webConfig;
        }

        private void LoadConfigurationSource(string webConfig)
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = webConfig
            };

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ConfigurationSource.Instance = new FileConfigurationSource(config);
        }

        private void LoadHostingEnvironment(string webConfig)
        {
            string physicalPath = Path.GetDirectoryName(webConfig);
            GenericHostingEnvironment.Instance = new NoneWebContextHostingEnvironment("/", physicalPath);
        }

        private void CreateDatabase()
        {
            string connectionString = GetConnectionString();

            RunSql(string.Format(@"
                    IF DB_ID('{0}') is not null
	                    begin
		                    ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
		                    DROP DATABASE [{0}]
	                    end

                    CREATE DATABASE [{0}]", GetDatabaseName(connectionString))
            );
        }

        private string GetDatabaseName(string connectionString)
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
            return builder.InitialCatalog;
        }

        private string GetConnectionString()
        {
            var connectionStringsSection = ConfigurationSource.Instance.Get<ConnectionStringsSection>("connectionStrings");
            return connectionStringsSection.ConnectionStrings["EPiServerDB"].ConnectionString;
        }

        private string GetConnectionStringToMaster()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(GetConnectionString());
            builder.InitialCatalog = "master";

            return builder.ToString();
        }

        private void RunSql(string statement)
        {
            using (SqlConnection connection = new SqlConnection(GetConnectionStringToMaster()))
            {
                try
                {
                    connection.Open();

                    using (var command = new SqlCommand(statement, connection))
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
    }
}
