---
type: "chapter"
book: "/optimizely/test-framework-for-net5"
chapter: "/part-2"

title: "Create a service"
preamble: "We have now managed to start the public preview of Optimizely CMS 12 in the test project, and it is much easier now than before. But before we have a base for that we need more functions."
---

To start Optimizely CMS 12, we used `WebApplicationFactory<TEntryPoint>` and this class is a good base that we can extend on to get a service that has the full responsibility to manage Optimizely CMS 12.

## First iteration

The first iteration of the service could be called `EpiserverEngineFirstIteration<TEntryPoint>` and lets it inherit from `WebApplicationFactory<TEntryPoint>`. We can move the code from the previous test case into a `Start` method.

```csharp
public class EpiserverEngineFirstIteration<TEntryPoint>
    : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
	private bool _started;

 	public void Start()
	{
		if(_started)
        {
            return;
        }

        var repository = Services.GetInstance(typeof(IContentRepository));
        
        if (repository == null)
        {
            throw new InvalidOperationException("Failed starting");
        }

        _started = true;
    }
}
```

## Create the database

An important function is that the database should be created automatically at each test session, a good place to add this is in the `Start` method, you can find the complete code in [EpiserverEngineFirstIteration.cs](https://github.com/loremipsumdonec/episerver-testframework/tree/main/posts/test_framework_for_net5/example/Lorem.Test/Services/EpiserverEngineFirstIteration.cs) file.

```csharp
public class EpiserverEngineFirstIteration<TEntryPoint>
    : WebApplicationFactory<TEntryPoint> where TEntryPoint : class
{
	private bool _started;

 	public void Start()
	{
		if(_started)
        {
            return;
        }

        CreateCmsDatabase();
        
        var repository = Services.GetInstance(typeof(IContentRepository));
        
        if (repository == null)
        {
            throw new InvalidOperationException("Failed starting");
        }

        _started = true;
    }
        
    private void CreateCmsDatabase() 
    {
        ...
    }
}
```

## Create a test case

To test `EpiserverEngineFirstIteration<TEntryPoint>`, create a new test case that that creates a page. Which is a pretty simple and basic function

```csharp
[Fact]
public async void StartWithEpiserverEngineFirstIteration_CreateStartPage_HasPage()
{
    EpiserverEngineFirstIteration<Startup> engine = new();
    engine.Start();

    await CreateUser("Administrator", "Administrator123#", "loremipsumdonec@supersecretpassword.io", Roles.Administrators);

    IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

    var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

    startPage.Name = "Start";
    startPage.Heading = "Welcome to Lorem";
    startPage.StartPublish = DateTime.Now;

    repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

    var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
    Assert.Single(pages);
}
```

You can find the above test in [EpiserverEngineTest.cs](https://github.com/loremipsumdonec/episerver-testframework/tree/main/posts/test_framework_for_net5/example/Lorem.Test/EpiserverEngineTest.cs) and if you run the test it should succeed. The test case also creates a user that makes it possible to launch the web application and log in to check.

![](./resources/episerver_engine.first_test_succeded.png)

## Run multiple test cases

To be able to run several test cases in a row and have a good speed of the test cases, you should only start Optimizely CMS once during each test session. You then delete the content that is in the database between each test case.

I prefer to delete the content before each test case. This will make it easier to troubleshoot any test cases that fail as the data will remain in the database.

> There is also another type of data that is not content and needs to be deleted between test cases. This is out of scope for this introduction, instead look in the file [CmsTestFramework.cs](https://github.com/loremipsumdonec/episerver-testframework/tree/main/src/main/Lorem.Testing.EPiServer.CMS/TestFrameworks/CmsTestFramework.cs) for a examples.

To keep the same instance running throughout the test session you can use [Shared Context between tests.](https://xunit.net/docs/shared-context) The following example uses `EpiserverEngineSencodIteration<TEntryPoint>` which deletes available content, check the file [EpiserverEngineSecondIteration.cs](https://github.com/loremipsumdonec/episerver-testframework/tree/main/posts/test_framework_for_net5/example/Lorem.Test/Services/EpiserverEngineSecondIteration.cs)

```csharp
[CollectionDefinition("Default")]
public class EpiserverEngineCollectionFixture
	: ICollectionFixture<EpiserverEngineSecondIteration<Startup>>
{
}

[Collection("Default")]
public class EpiserverEngineTestWithCollection
{
	private EpiserverEngineSecondIteration<Startup> _engine;

    public EpiserverEngineTestWithCollection(EpiserverEngineSecondIteration<Startup> engine) 
    {
        _engine = engine;
        _engine.Start();
    }
    
    [Theory]
    [InlineData(1)]
    [InlineData(2)]
    [InlineData(3)]
    [InlineData(4)]
    public async void StartWithEpiserverEngineSecondIteration_CreateStartPage_HasPage(int _)
    {
    	await CreateUser(...);

		IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

        var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

        startPage.Name = "Start";
        startPage.Heading = "Welcome to Lorem";
        startPage.StartPublish = DateTime.Now;

        repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

        var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
        Assert.Single(pages);
	}
}
```

![](./resources/episerver_engine_second_multiple_tests.png)

## Conclusion

Now we have gone through some of the steps to start up the public preview of Optimizely CMS 12, and it is much easier and better than before.

Another advantage is that there is also no major difference from CMS 11 in .NET framework. Which means that it will be relatively easy to migrate your test cases from CMS 11 to CMS 12. Of course, some adjustments will be required, but it feels like it will be manageable.
