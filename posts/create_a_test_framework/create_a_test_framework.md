---
date: "2021-08-09"
type: "book"
book: "/optimizely/create-a-test-framework"
theme: "pink"
state: "done"
tags: ["optimizely"]

repository_url: "https://github.com/loremipsumdonec/episerver-testframework"
repository_name: "episerver-testframework"
repository_base: "https://github.com/loremipsumdonec/episerver-testframework/blob/main/posts/create_a_test_framework"

title: "Test framework for Optimizely CMS 11"
preamble: "In this post, I intend to show how to build a test framework for Optimizely CMS and how to write different types of integration tests to check its implementation."

---

As many probably know or have heard, it is good to perform some type of automatic testing to check that important functions have not stopped working because of a recent change in the source code. There is a lack for this type of information on the internet so I thought I would simply contribute what I have learned.

##  The goal

 The goal of this post is to show how you can start Episerver in a testing process and share some thoughts (and code) on how to then build a testing framework for integration testing. The code and examples are available on [Github](https://github.com/loremipsumdonec/episerver-testframework).
