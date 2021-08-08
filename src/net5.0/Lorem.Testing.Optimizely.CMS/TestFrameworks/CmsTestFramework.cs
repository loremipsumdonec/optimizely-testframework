using Lorem.Testing.Optimizely.CMS.Commands;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

namespace Lorem.Testing.Optimizely.CMS.TestFrameworks
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

        public void BeforeInitialize(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString(_connectionStringName);

            CreateCmsDatabase();
        }

        public void AfterInitialize()
        {
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
}
