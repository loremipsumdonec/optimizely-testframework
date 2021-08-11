---
date: "2021-08-11"
type: "book"
book: "/optimizely/test-framework-for-net5"
theme: "blue"
state: "done"
tags: ["optimizely"]

repository_url: "https://github.com/loremipsumdonec/episerver-testframework"
repository_name: "episerver-testframework"
repository_base: "https://github.com/loremipsumdonec/episerver-testframework/blob/main/posts/test_framework_for_net5"

title: "Testing with Optimizely CMS 12 preview"
preamble: "Now when Optimizely has released a public preview of Optimizely CMS 12 that’s built with .NET 5, it's time to start exploring how to perform integration testing."
---

> Note that it is still a public preview so there will certainly be some changes until the official release that comes later this year.

## Integration testing in .NET 5

In .NET 5, it is much easier to run integration tests that test from a client's perspective (visit an url). This is more difficult to achieve in the .NET Framework that Optimizely CMS 11 and older, as you usually must start a process in IIS. Which means that you get two instances of Optimizely CMS where one is run in an IIS and the other instance is run in the test process. This leads to a lot of other types of problems such as cache management. It is not impossible to achieve but in my opinion is not worth the effort.

> It is possible to test controllers but I think it increases the complexity of the test cases. I prefer have lightweight controllers and not have any direct logic in these, instead they uses other services and which makes it easier to test (and maintain).

If you want to get started with integration testing, Microsoft has posted information this topic, see [Integration tests in ASP.NET Core](https://docs.microsoft.com/en-us/aspnet/core/test/integration-tests?view=aspnetcore-5.0).

## The goal

The goal is to show the steps how to start Optimizely CMS 12 in a test project.

