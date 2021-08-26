---
date: "2021-08-26"
type: "book"
book: "/optimizely/test-framework-released"
theme: "green"
state: "in_progress"
tags: [""]

repository_url: "https://github.com/loremipsumdonec/optimizely-testframework"
repository_name: "optimizely-testframework"
repository_base: "https://github.com/loremipsumdonec/optimizely-testframework/blob/main/posts/test_framework_released"

title: "Released a test framework"
preamble: "In the recent weeks, I have taken the time to build a test framework that can be used for integration testing of Optimizely CMS. Hopefully it simplifies for you to begin testing with Optimizely CMS."
---

The framework simplifies the process of managing pages, blocks, and files. Before each test session the framework will recreate the database and delete any files in App_Data/blobs folder. So that the test session begins with a clean environment, and between each test case it will delete the contents in the database. You can now get the following packages from [nuget.optimizely.com](https://nuget.optimizely.com/)

- [Lorem.Test.Framework.Optimizely.CMS](https://nuget.optimizely.com/package/?id=Lorem.Test.Framework.Optimizely.CMS)
- [Lorem.Test.Framework.Optimizely.SearchAndNavigation](https://nuget.optimizely.com/package/?id=Lorem.Test.Framework.Optimizely.SearchAndNavigation)

**Example of a test case**

```csharp
[Fact]
public void GetBreadcrumbs_AllPagesVisibileInBreadcrum_HasExpectedBreadcrumbs()
{
    Fixture.CreatePath<StandardPage>(4, p =>
    {
        p.VisibleInBreadcrum = true;
        p.Heading = IpsumGenerator.Generate(3);
    });

    GetBreadcrumbs query = new GetBreadcrumbs(
        Fixture.Contents[0].ContentLink,
        Fixture.Contents.Last()
    );

    var model = Dispatcher.Dispatch<BreadcrumbsModel>(query);
    Assert.Equal(4, model.Breadcrumbs.Count);
}

```

I have not written a complete documentation of all the features yet, so if you need examples take a look in the tests used to test the framework.

* [PageBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/tree/main/src/net48/Lorem.Test.Framework.Optimizely.CMS.Test/Builders/PageBuilderTests.cs)
* [BlockBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/tree/main/src/net48/Lorem.Test.Framework.Optimizely.CMS.Test/Builders/BlockBuilderTests.cs)
* [MediaBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/tree/main/src/net48/Lorem.Test.Framework.Optimizely.CMS.Test/Builders/MediaBuilderTests.cs)
* [ContentBuilderTests](https://github.com/loremipsumdonec/optimizely-testframework/tree/main/src/net48/Lorem.Test.Framework.Optimizely.CMS.Test/Builders/ContentBuilderTests.cs)

 To get started, visit the Github repository https://github.com/loremipsumdonec/optimizely-testframework. If you have any problems or questions, post an issue in the project's github.
