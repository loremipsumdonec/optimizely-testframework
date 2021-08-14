# Testing framework for Optimizely CMS

This is a project where I intend to gather information on how to work with integration testing for Optimizely CMS. I will create posts/tutorials and also add examples of code, as well as a framework that you can use to hopefully simplify it a bit.

## Posts

I have written a post that describes how to start with integration testing for Optimizely CMS 11.

- https://www.tiff.se/optimizely/create-a-test-framework

## Framework

[![optimizely cms 11](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_11.yml/badge.svg)](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_11.yml) [![optimizely cms 12](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_12.yml/badge.svg)](https://github.com/loremipsumdonec/episerver-testframework/actions/workflows/test_optimizely_cms_12.yml)

I have started a small framework that hopefully simplifies the process of starting and creating data in Optimizely CMS  so that you can then focus on writing clear test cases.

### Getting started

#### Install

The framework is currently not available in any nuget feed.

#### Access to Web.config

The framework needs to have access to the same _Web.config_ used by the web project. The easiest way is to add a link in the test project that points to _Web.config_ in the web project. Edit the project file (_*. csproj_) and add the `ItemGroup` element below where `Include` has the relative path to _Web.config_ in the web project.

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

If you have done it correctly, it will look like below. Build the test project and check that _Web.config_ is in the output directory.

#### Add the engine and fixture

The next step is that you need to implement two classes that inherit from `Lorem.Testing.Optimizely.CMS.Engine` and `Lorem.Testing.Optimizely.CMS.Fixture`.  The class `Lorem.Testing.Optimizely.CMS.Engine` is responsible for the the startup of Optimizely CMS. While  `Lorem.Testing.Optimizely.CMS.Fixture` will be the primary service that will be used in the test cases.

Below is an example of a class that inherits from `Lorem.Testing.Optimizely.CMS.Engine` that uses the `CmsTestModule`, which is responsible for setting up the database and clearing the content.

```csharp
public class DefaultEngine : Lorem.Testing.Optimizely.CMS.Engine
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

> As a small safe guard that you understand that `CmsModule` will recreate the database and delete files, you need to set the following properties on `CmsModule` to `true` `IamAwareThatTheDatabaseWillBeDeletedAndReCreated` and `IamAwareThatTheFilesAndFoldersAtAppDataPathWillBeDeleted`.

The class that inherits from `Lorem.Testing.Optimizely.CMS.Fixture` is responsible for configuration such as languages to be used, see this as global settings. Each test case will then have its own instance.

```csharp
public class DefaultFixture : Lorem.Testing.Optimizely.CMS.Fixture
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

```csharp
[CollectionDefinition("Default")]
public class DefaultEngineCollectionFixture 
    : ICollectionFixture<DefaultEngine>
{
}
```

```
[Collection("Default")]
public class ExploratoryTests
{
    public ExploratoryTests(DefaultEngine engine)
    {
    	Fixture = new DefaultFixture(engine);
    }

	public Fixture Fixture { get; set; }

    [Fact]
    public void ASimpleTest()
    {
    	Fixture.Create<StartPage>();
    	
        var repository = Fixture.GetInstance<IContentLoader>();
        Assert.Single(repository.GetChildren<StartPage>(ContentReference.RootPage));
    }
}
```

