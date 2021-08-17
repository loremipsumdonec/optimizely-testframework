# Testing framework for Optimizely CMS

This is a project where I intend to gather information on how to work with integration testing for Optimizely CMS. I will create posts/tutorials and also add examples of code, as well as a framework that you can use to hopefully simplify it a bit.

## Posts

I have written a post that describes how to start with integration testing for Optimizely CMS 11.

- https://www.tiff.se/optimizely/create-a-test-framework

## Framework

[![optimizely cms 11](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_11.yml/badge.svg)](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_11.yml) [![optimizely cms 12](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_12.yml/badge.svg)](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_12.yml)

I have created a small framework that hopefully simplifies the process to start with integration testing for Optimizely CMS. Below is an example of a test case that tests a `IBreadCrumbService` and verifies that it ignores pages that has `VisibleInBreadCrumb=false`.

```csharp
[Collection("Default")]
public class BreadcrumbsServiceTest
{
    public BreadcrumbsServiceTest(DefaultEngine engine) 
    {
        Fixture = new DefaultFixture(engine);
    }
    
    public DefaultFixture Fixture {get;}
        
    [Fact]
    public void GetBreadCrumbs_OnePageInPathNotVisibleInBreadCrumb_HasExpectedCount()
    {
        int totalDepth = 3;
        Fixture.CreatePath<ArticlePage>(totalDepth, (p, depth) => p.VisibleInBreadCrumb = depth != 1);

        var service = Fixture.GetInstance<IBreadCrumbService>();
        
        var breadcrumbs = service.GetBreadCrumbs(pages.Last());

        Assert.Equal(totalDepth - 1, breadcrumbs.Count);
    }
}
```

The test case will create three pages in the tree structure below and disable `VisibleInBreadCrumb` on `ArticlePage` at depth 1.

![readme_create_path_example](.\posts\resources\readme_create_path_example.png)

In addition to providing support for creating content such as pages, blocks and uploading files, it is also possible to replace  services with test doubles. For example, if you use [Moq](https://github.com/moq/moq4), you can temporarily replace a service like `IContentRepository`. 

```csharp
[Collection("Default")]
public class FixtureNestedContextTests
{
    public FixtureNestedContextTests(DefaultEngine engine) 
    {
        Fixture = new DefaultFixture(engine);
    }
    
    public DefaultFixture Fixture {get;}
        
    [Fact]
    public void ReplaceServiceWith_WithUsing_GetChildrenAfterDispose()
    {
        var mock = new Mock<IContentRepository>();

        mock.Setup(
            r => r.GetChildren<StartPage>(It.IsAny<ContentReference>())
        ).Throws(new FileNotFoundException("Only for testing"));

        Fixture.Create<StartPage>();

        using (Fixture.ReplaceServiceWith<IContentRepository>(mock.Object))
        {
            var testDoubleRepository = ServiceLocator.Current.GetInstance<IContentRepository>();

            Assert.Throws<FileNotFoundException>(
                () => testDoubleRepository.GetChildren<StartPage>(ContentReference.RootPage)
            );
        }

        var repository = ServiceLocator.Current.GetInstance<IContentRepository>();
        var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);

        Assert.Single(pages);
    }
}
```

### Getting started

#### Install

> The framework is currently not available in any public nuget feed. 

Create a test project and install the packages listed below, need to be same version as web project. Then install `Lorem.Test.Optimizely.CMS` and add a project reference to _web project_.

- EPiServer.CMS.Core
- EPiServer.CMS.AspNet
- EPiServer.CMS.UI.Core
- EPiServer.CMS.UI.AspNetIdentity
- EPiServer.Framework

#### Access to Web.config

The framework needs to have access to the same _Web.config_ used by the web project. The easiest way is to add a link in the test project that points to _Web.config_. Edit the project file (_*. csproj_) and add an `ItemGroup` element where attribute `Include` has the relative path to _Web.config_. Build the test project and check that _Web.config_ is in the output directory.

```xml
<Project>
	...
    <ItemGroup>
        <None Include="..\Lorem\Web.config">
            <Link>Web.config</Link>
            <CopyToOutputDirectory>Always</CopyToOutputDirectory>
        </None>
    </ItemGroup>
</Project>
```

#### Create an engine

The next step is to implement a class that inherits from `Lorem.Test.Optimizely.CMS.Engine`. This class will be responsible for the startup of Optimizely CMS. 

Below is an example of a class that inherits from `Lorem.Test.Optimizely.CMS.Engine` that uses the `CmsTestModule`, which is responsible for setting up the database and clearing the content.

```csharp
public class DefaultEngine : Lorem.Test.Optimizely.CMS.Engine
{
    public DefaultEngine()
    {
    	Add(new CmsTestModule()
            {
                IamAwareThatTheDatabaseWillBeDeletedAndReCreated = true,
                IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted = true
            });
    }
}
```

By default, `CmsTestModule` will use the information contained in _Web.config_. If you have a relative app data path in _Web.config_, it will be from the test project's output directory.

> As a small safeguard so that you understand that `CmsModule` will recreate the database and delete files, you need to set the following properties on `CmsModule` to `true` `IamAwareThatTheDatabaseWillBeDeletedAndReCreated` and `IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted`.

Also check so that the *Web.config* has the following configuration, this will make Optimizely CMS create the necessary tables in the database.

```xml
<episerver.framework createDatabaseSchema="true" updateDatabaseSchema="true">
```
##### With Search & Navigation

If your project uses Search & Navigation, then you will need to install `Lorem.Test.Optimizely.SearchAndNavigation` and add the following packages to the test project.

* EPiServer.Find.Cms

Then you can activate the module by adding `SearchAndNavigationTestModule` in `Lorem.Test.Optimizely.CMS.Engine`.

```csharp
public class DefaultEngine : Lorem.Test.Optimizely.CMS.Engine
{
    public DefaultEngine()
    {
    	Add(new CmsTestModule()
            {
                IamAwareThatTheDatabaseWillBeDeletedAndReCreated = true,
                IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted = true
            });
        
        Add(SearchAndNavigationTestModule());
    }
}
```

#### Create the fixture

The next class that needs to be created is the fixture, this class needs to inherit from `Lorem.Test.Optimizely.CMS.Fixture`. This class will be responsible for the configuration such as languages, builders etc. Each test case will then have its own instance.

```csharp
public class DefaultFixture : Lorem.Test.Optimizely.CMS.Fixture
{
    public DefaultFixture(IEngine engine)
    	: base(engine)
    {
    	Cultures.Add(CultureInfo.GetCultureInfo("en"));
        Cultures.Add(CultureInfo.GetCultureInfo("sv"));
    	
        RegisterBuilder<SiteDefinition>(s => {
            s.Name = "Lorem";
            s.SiteUrl = new Uri("http://localhost:65099");
        });
    	
    	Start();
    }
}
```

#### Create a test case

If you use xUnit, you should use [Shared Context between Tests](https://xunit.net/docs/shared-context) for `Lorem.Testing.Optimizely.CMS.Engine` so that it only starts up once in each test session.

> It is not optimal to start and stop Optimizely CMS between each test case, read more about this in the chapter [Fixing the problems](https://www.tiff.se/optimizely/create-a-test-framework/part-4)

Below is an exempel of an test case using the collection fixture feature in xUnit.

```csharp
[CollectionDefinition("Default")]
public class DefaultEngineCollectionFixture 
    : ICollectionFixture<DefaultEngine>
{
}
```



```csharp
[Collection("Default")]
public class MyFirstIntegrationTests
{
    public MyFirstIntegrationTests(DefaultEngine engine)
    {
    	Fixture = new DefaultFixture(engine);
    }

	public Fixture Fixture { get; }

    [Fact]
    public void CreateAStartPage_StartPageExists()
    {
    	Fixture.Create<StartPage>();
    	
        var repository = Fixture.GetInstance<IContentLoader>();
        Assert.Single(repository.GetChildren<StartPage>(ContentReference.RootPage));
    }
}
```

## Examples

The following section shows examples of regular configurations for `Lorem.Test.Optimizely.CMS.Engine` and `Lorem.Test.Framework.Optimizely.CMS.Fixture`.

### Default Engine

```csharp
public class DefaultEngine : Lorem.Test.Optimizely.CMS.Engine
{
    public DefaultEngine()
    {
    	Add(new CmsTestModule()
            {
                IamAwareThatTheDatabaseWillBeDeletedAndReCreated = true,
                IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted = true
            });
    }
}
```

### Engine with support for Search and Navigation

For this configuration you will need to install the nuget `Lorem.Testing.Optimizely.SearchAndNavigation`.

```csharp
public class DefaultEngine : Lorem.Test.Optimizely.CMS.Engine
{
    public DefaultEngine()
    {
    	Add(new CmsTestModule()
            {
                IamAwareThatTheDatabaseWillBeDeletedAndReCreated = true,
                IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted = true
            });
        
        Add(new SearchAndNavigationTestModule());
    }
}
```

### Default fixture

```csharp
public class DefaultFixture
    : Fixture
{
    public DefaultFixture(IEngine engine)
        : base(engine)
    {
        Cultures.Add(CultureInfo.GetCultureInfo("en"));

        RegisterBuilder<SiteDefinition>(s => {
            s.Name = "Lorem";
            s.SiteUrl = new Uri("http://localhost:65099");
        });

        Start();
    }
}
```

### Fixture creating a user
```csharp
public class ExploratoryFixture
    : Fixture
{
    public ExploratoryFixture(IEngine engine)
        : base(engine)
    {
        Cultures.Add(CultureInfo.GetCultureInfo("en"));

        RegisterBuilder<SiteDefinition>(s => {
            s.Name = "Lorem";
            s.SiteUrl = new Uri("http://localhost:65099");
        });

        Start();
        
        CreateUser(
            "Administrator",
            "Administrator123!",
            "admin@supersecretpassword.io",
            "WebAdmins", "Administrators"
        );
    }
}
```


### Functions

If you need examples of functions thats available check the tests in the test projects.

* [PageBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/blob/main/src/net48/Lorem.Test.Optimizely.CMS.Test/Builders/PageBuilderTests.cs)
* [BlockBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/blob/main/src/net48/Lorem.Test.Optimizely.CMS.Test/Builders/BlockBuilderTests.cs)
* [MediaBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/blob/main/src/net48/Lorem.Test.Optimizely.CMS.Test/Builders/MediaBuilderTests.cs)
* [ContentBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/blob/main/src/net48/Lorem.Test.Optimizely.CMS.Test/Builders/ContentBuilderTests.cs)

