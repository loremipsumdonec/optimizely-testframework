using EPiServer;
using EPiServer.Framework;
using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Hosting;
using Lorem.Test.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;
using Xunit;

namespace Lorem.Test
{
    public class StartEpiserverTest
    {
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
            var connectionStringsSection = ConfigurationSource.Instance.Get<ConnectionStringsSection>("connectionStrings");
            string connectionString = connectionStringsSection.ConnectionStrings["EPiServerDB"].ConnectionString;
            
            string createDatabaseSqlStatement = string.Format(@"
                    IF DB_ID('{0}') is not null
	                    begin
		                    ALTER DATABASE [{0}] SET  SINGLE_USER WITH ROLLBACK IMMEDIATE
		                    DROP DATABASE [{0}]
	                    end

                    CREATE DATABASE [{0}]", GetDatabaseName(connectionString));

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

        [Fact]
        public void StartEpiserver_FirstTestCase_IContentRepositoryIsNotNull() 
        {
            List<IInitializableModule> modules = null;
            var assemblies = new AssemblyList(true).AllowedAssemblies;

            var engine = new InitializationEngine(modules, HostType.TestFramework, assemblies);
            engine.Initialize();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            Assert.NotNull(repository);
        }

        [Fact]
        public void StartEpiserver_LoadWebConfig_IContentRepositoryIsNotNull()
        {
            string webConfig = GetWebConfig();
            LoadConfigurationSource(webConfig);

            List<IInitializableModule> modules = null;
            var assemblies = new AssemblyList(true).AllowedAssemblies;

            var engine = new InitializationEngine(modules, HostType.TestFramework, assemblies);
            engine.Initialize();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            Assert.NotNull(repository);
        }

        [Fact]
        public void StartEpiserver_IContentRepositoryIsNotNull()
        {
            string webConfig = GetWebConfig();
            LoadConfigurationSource(webConfig);
            LoadHostingEnvironment(webConfig);
            CreateDatabase();

            List<IInitializableModule> modules = null;
            var assemblies = new AssemblyList(true).AllowedAssemblies;

            var engine = new InitializationEngine(modules, HostType.TestFramework, assemblies);
            engine.Initialize();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            Assert.NotNull(repository);
        }
    }
}
