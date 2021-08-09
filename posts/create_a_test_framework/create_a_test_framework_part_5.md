---
type: "chapter"
book: "/optimizely/create-a-test-framework"
chapter: "/part-5"

title: "The structure of a test case"
preamble: "This is the last chapter and here I will raise my thoughts about the structure of the test cases and show some examples."
---

## How do you want to write the test cases?

When you do integration testing you usually need a framework to control the system you are testing, and it is no different with Episerver. We already have a base for *start* and *stop* and now we need to simplify the process of creating data, pages, blocks etc.

If we look at the previous test case that we have worked with we can see that it contains too much information that we don´t really need or want to see.

> It's not really a test either, but it contains the basic of what we need to write to create a page.

```csharp
[Fact]
public void StartWithEpiserverEngineSecondIteration_CreatePage_SinglePageExists()
{
    var engine = EpiserverEngineSecondIteration.GetInstance();
    engine.Start();

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

If we play with the idea that this is a "_real test_" that should check that it succeeds in creating a page. Then we are only interested in creating a `StartPage` and not which _name_ or _heading_ the page has. Then we would probably want something like this.


```csharp
[Fact]
public void GetChildren_OneChild_HasOneChild()
{
    Fixture.Create<StartPage>();

    var repository = Fixture.GetInstance<IContentRepository>();
    var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);

    Assert.Single(pages);
}
```

Now the test case does not contain that much setup code and we do not need to think about starting Episerver, it is something that `Fixture` is responsible for and we can focus on the test itself.

If we continue with another example where you need to create several pages, you may want a fluent API that may look like the following.

 ```csharp
[Fact]
public void GetChildren_CreateManyChildren_HasExpectedChildren()
{
    int expected = 10;

    var pages = Fixture.Create<StartPage>()
        .CreateMany<ArticlePage>(expected);

    var repository = Fixture.GetInstance<IContentRepository>();
    var children = repository.GetChildren<ArticlePage>(pages.First().ContentLink);

    Assert.Equal(expected, children.Count());
}
 ```

It is important that you and your team decide how you want to write your test cases and at what level of ambition you want. Then build a service that will enable you to work in that way.

## A real test case

Now it is time to show a real test case that tests an implementation of a bread crumb function.

> The test case uses the [xUnit](https://xunit.net/) function [shared test context](https://xunit.net/docs/shared-context)

```csharp
[Collection("Default")]
public class BreadcrumbsServiceTest
{
    public BreadcrumbsServiceTest(DefaultEpiserverEngine engine) 
    {
        Fixture = new DefaultEpiserverFixture(engine);
    }
    
    public EpiserverFixture Fixture {get; set;}
        
    [Fact]
    public void GetBreadCrumbs_OnePageInPathNotVisibleInBreadCrumb_HasExpectedCount()
    {
        var pages = Fixture.Create<StartPage>(p => p.VisibleInBreadCrumb = true)
            .Create<ArticlePage>(p => p.VisibleInBreadCrumb = false)
            .Create(p => p.VisibleInBreadCrumb = true);

        var service = Fixture.GetInstance<IBreadCrumbService>();
        var breadcrumbs = service.GetBreadCrumbs(pages.Last());

        Assert.Equal(pages.Count() - 1, breadcrumbs.Count);
    }
}

```

## Benefits with a testing framework

Most people probably know the benefits of building automated test cases.

In smaller projects I use the test framework to speed up the development of functions, as you do not need to jump between Visual studio and a browser to debug a function. With a test framework, it will be possible to activate the function with a test case and then stay in the Visual studio. Should there be any problem, I can still start Episerver as a web application and use the same data.

I also use the test framework to investigate different options for implementing a feature, and build a demo environment where you want to show all functions of the system.

### Quirks

There will be situations where you need to customize or simply disable functions in Episerver to make it possible to start the test framework, for example if you use _Episerver Forms_ you need to remove the following initialization modules when running as a test framework.

- EPiServer.Forms.InitializationModule
- EPiServer.Forms.EditView.InitializationModule

There are several modules that are only adapted to run as a web application. You will need to troubleshoot Episerver and learn how Episerver works in depth, but that's where you use .NET decompilers.

As long as you have the will and the time, there should be no problems.

## Conclusion

I raised some thoughts about the structure of the test cases and showed an example of a real test case that tests its own implementation. All code is available in the repository and use it as needed and adapt it to your way of working.

I'm not sure when I'll write the next part. 
