---
url: "/episerver/create-a-test-framework/"
date: "2021-07-27"
book: "/episerver/create-a-test-framework/"
type: "book"
tags: ["optimizely",  "episerver", "integration testing"]

excerpt: "In this post, I intend to show how to build a testframework for Episerver Content Cloud and how to write different types of integration tests to check its implementation."

title: "Create a test framework for Content Cloud"
preamble: "In this post, I intend to show how to build a test framework for Episerver Content Cloud and how to write different types of integration tests to check its implementation."

---

As many probably know or have heard, it is good to perform some type of automatic testing to check that important functions have not stopped working because of a recent change in the source code. There is a lack for this type of information on the internet so I thought I would simply contribute what I have learned.

##  The goal

 The goal of this post is to show how you can start Episerver in a testing process and share some thoughts (and code) on how to then build a testing framework for integration testing. The code and examples are available on [Github](https://github.com/loremipsumdonec/episerver-testframework).