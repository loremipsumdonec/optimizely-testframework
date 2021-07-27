using EPiServer.Framework;
using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using EPiServer.Web.Hosting;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.IO;

namespace Lorem.Test.Services
{
    public class EpiserverEngineSecondIteration
    {
        private static EpiserverEngineSecondIteration _singelton;
        private InitializationEngine _engine;

        public static EpiserverEngineSecondIteration GetInstance()
        {
            if(_singelton == null)
            {
                _singelton = new EpiserverEngineSecondIteration();
            }

            return _singelton;
        }

        private EpiserverEngineSecondIteration() 
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

                TakeBackup();
            }
        }

        private void Reset()
        {
            RestoreWithBackup();
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

        private string GetDatabaseName()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(GetConnectionString());
            return builder.InitialCatalog;
        }

        private string GetConnectionStringToMaster()
        {
            SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(GetConnectionString());
            builder.InitialCatalog = "master";

            return builder.ToString();
        }

        private void RestoreWithBackup()
        {
            string databaseName = GetDatabaseName();
            string backup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "episerver_engine.bak");

            RunSql(string.Format(@"
                    ALTER DATABASE [{0}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;",
                databaseName,
                backup)
            );

            RunSql(string.Format(@"
                    RESTORE DATABASE[{0}] FROM  DISK = N'{1}' WITH  FILE = 1, NOUNLOAD, REPLACE, STATS = 5",
                databaseName,
                backup)
            );

            RunSql(string.Format(@"
                    ALTER DATABASE [{0}] SET MULTI_USER;",
                    databaseName,
                    backup)
                );
        }

        private void TakeBackup()
        {
            string databaseName = GetDatabaseName();
            string backup = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "episerver_engine.bak");

            if(File.Exists(backup))
            {
                File.Delete(backup);
            }

            RunSql(string.Format(@"
                    BACKUP DATABASE [{0}] 
                    TO  DISK = N'{1}' 
                    WITH NOFORMAT, NOINIT,  NAME = N'{0}', SKIP, NOREWIND, NOUNLOAD,  STATS = 10",
                    databaseName,
                    backup)
                );
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
