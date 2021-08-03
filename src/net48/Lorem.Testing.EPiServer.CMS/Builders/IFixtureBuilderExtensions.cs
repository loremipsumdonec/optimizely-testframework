﻿using EPiServer.Core;
using System;
using System.Collections.Generic;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public static class IFixtureBuilderExtensions
    {
        #region Block

        public static IBlockBuilder<TBlockType> CreateBlock<TBlockType>(this IFixtureBuilder builder, Action<TBlockType> build = null)
            where TBlockType : BlockData => new BlockBuilder<TBlockType>(builder.Fixture).CreateBlock(build);


        #endregion

        #region Page

        public static IPageBuilder<TPageType> Create<TPageType>(this IFixtureBuilder builder, Action<TPageType> build = null)
            where TPageType : PageData => new PageBuilder<TPageType>(builder.Fixture).Create(build);

        public static IPageBuilder<TPageType> CreateMany<TPageType>(this IFixtureBuilder builder, int total, Action<TPageType, int> build = null)
            where TPageType : PageData => new PageBuilder<TPageType>(builder.Fixture).CreateMany(total, build);

        public static IPageBuilder<TPageType> CreatePath<TPageType>(this IFixtureBuilder builder, int depth, Action<TPageType> build = null)
            where TPageType : PageData => new PageBuilder<TPageType>(builder.Fixture).CreatePath(depth, build);

        #endregion

        #region Media

        #endregion

        #region Content

        public static IContentBuilder<T> Publish<T>(this IFixtureBuilder<T> builder)
            where T : IContent => new ContentBuilder<T>(builder.Fixture).Publish();

        public static IContentBuilder<T> Expire<T>(this IFixtureBuilder<T> builder)
            where T : IContent => new ContentBuilder<T>(builder.Fixture).Expire();

        public static IContentBuilder<T> Delete<T>(this IFixtureBuilder<T> builder)
            where T : IContent => new ContentBuilder<T>(builder.Fixture).Delete();

        public static void ForceDelete<T>(this IFixtureBuilder<T> builder)
            where T : IContent => new ContentBuilder<T>(builder.Fixture).ForceDelete();

        public static IContentBuilder<T> Move<T>(this IFixtureBuilder<T> builder, ContentReference destination)
            where T : IContent => new ContentBuilder<T>(builder.Fixture).Move(destination);

        #endregion
    }
}