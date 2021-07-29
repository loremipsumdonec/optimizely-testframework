---
url: "/episerver/create-a-test-framework/part-2/"
date: "2021-07-27"
book: "/episerver/create-a-test-framework/"
type: "chapter"

title: "Start Episerver"
preamble: "This chapter will show how you can start Episerver in the test project that was created in the previous chapter."
---

## Create a test case

To start Episerver, you will need to create an instance of the class `EPiServer.Framework.Initialization.InitializationEngine` where you flag that it should run with `HostType.TestFramework` then you call the method `Initialize` to start up Episerver. 

To test this, we can start by setting up a simple test case where we use the code below. The goal of the test is that it can retrieve `IContentRepository`, you can also find the code in the file [StartEpiserverTest.cs](https://github.com/loremipsumdonec/episerver-testframework/blob/main/Lorem.Test/StartEpiserverTest.cs)

```csharp
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
```

When you run the above a `System.Data.SqlClient.SqlException` will be thrown which is due to Episerver not being able to connect to a database. Which is not so strange as the test process does not load _Web.config_.

> The test process is more like a console program and expects an App.config instead of a Web.config

## Use the Web.config

There are several ways to handle the configuration of Episerver. One is to set up an _initialization module_ that configures `DataAccessOptions` while the other is to control which configuration file to use, which can be done with ` EPiServer.Framework.Configuration.ConfigurationSource`. I prefer the latter option and below is an example of what it might look like.

```csharp
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
```

But when you run the code you will get a new error message.

```
EPiServer.Framework.Initialization.InitializationException : Initialize action failed for Initialize on class EPiServer.Framework.Initialization.Internal.VirtualPathProviderInitialization, EPiServer.Framework.AspNet, Version=11.15.1.0, Culture=neutral, PublicKeyToken=8fe83dea738b45b7
```

## Create a hosting environment

To solve the error message, you need to add a new class that implements `EPiServer.Web.Hosting.IHostingEnvironment` and register it by setting the `EPiServer.Web.Hosting.GenericHostingEnvironment.Instance`. Episerver uses this to access some environment variables.

> This is a class that I unfortunately do not know the origin of, but it occurs in several other projects where you run Episerver as a console program and if you do a [search](https://www.google.com/search?q=NoneWebContextHostingEnvironment) you will find some.

```csharp
public class NoneWebContextHostingEnvironment
	: IHostingEnvironment
{
    public NoneWebContextHostingEnvironment(string virtualPath, string physicalPath)
    {
    	ApplicationPhysicalPath = physicalPath;
    	ApplicationVirtualPath = virtualPath;
    }

    public string ApplicationID { get; set; }

    public string ApplicationPhysicalPath { get; set; }

    public string ApplicationVirtualPath { get; set; }

    public VirtualPathProvider VirtualPathProvider { get; private set; }

    public string MapPath(string virtualPath)
    {
    	virtualPath = virtualPath.Trim(new char[] { ' ', '~', '/' }).Replace('/', '\\');
    	return Path.Combine(ApplicationPhysicalPath, virtualPath);
    }

    public void RegisterVirtualPathProvider(VirtualPathProvider virtualPathProvider)
    {
    	VirtualPathProvider = virtualPathProvider;
    }
}
```

The code below creates a new instance of `NoneWebContextHostingEnvironment` and registers it on ` EPiServer.Web.Hosting.GenericHostingEnvironment.Instance`. When you run the code, you will get a new error message that I think most Episerver developers has seen.

```csharp
private string GetWebConfig()
{
	...
}

private void LoadConfigurationSource(string webConfig)
{
	...
}

private void LoadHostingEnvironment(string webConfig)
{
	string physicalPath = Path.GetDirectoryName(webConfig);
	GenericHostingEnvironment.Instance = new NoneWebContextHostingEnvironment("/", physicalPath);
}

[Fact]
public void StartEpiserver()
{
    string webConfig = GetWebConfig();
    LoadConfigurationSource(webConfig);
    LoadHostingEnvironment(webConfig);

	...
}
```

>  There is no database that Episerver can connect to.

```bash
System.Data.SqlClient.SqlException : Cannot open database "Lorem" requested by the login. The login failed.
```

## Create the database automatically

The database is not something that you should have to create manually, it must be created automatically every time you start a new test session. This is quite easy to do as we only need to create an empty database. 

Before we run the final test case, we need to update the _Web.config_ with the following configuration so that Episerver will automatically create the tables.

```xml
<episerver.framework createDatabaseSchema="true" updateDatabaseSchema="true">
```

```csharp
private string GetWebConfig()
{
	...
}

private void LoadConfigurationSource(string webConfig)
{
	...
}

private void LoadHostingEnvironment(string webConfig)
{
	...
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
```

If you run the code the test case will now succeed, which will indicate that we have successfully started Episerver in the test project. You can find the complete code in the file [StartEpiserverTest.cs](https://github.com/loremipsumdonec/episerver-testframework/Lorem.Test/StartEpiserverTest.cs).

![episerver started](./resources/test_project_episerver_started.png)

## Conclusion

We have now managed to start Episerver in the test project, but the code can not be used to build test cases. In the next chapter I will show how to simplify the startup of Episerver by creating some type of service so we can use it in multiple test cases.
