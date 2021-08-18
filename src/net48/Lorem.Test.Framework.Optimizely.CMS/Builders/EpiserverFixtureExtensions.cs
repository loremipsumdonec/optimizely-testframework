using EPiServer.Core;
using EPiServer.ServiceLocation;
using Lorem.Test.Framework.Optimizely.CMS.Services;
using System;
using System.Globalization;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public static class EpiserverFixtureExtensions
    {
        #region Site

        public static ISiteBuilder<T> CreateSite<T>(this Fixture fixture, Action<T> build = null) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).Create<T>(build).CreateSite(fixture);
        }

        public static ISiteBuilder<T> CreateSite<T>(this Fixture fixture, string name, string url, Action<T> build = null) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).Create<T>(build).CreateSite(fixture, name, url);
        }

        public static ISiteBuilder<T> CreateSite<T>(this Fixture fixture, CultureInfo[] cultures,  Action<T> build = null) where T : PageData
        {
            fixture.Cultures.Clear();
            fixture.Cultures.AddRange(cultures);
            fixture.Reset();

            return new PageBuilder<T>(fixture).Create<T>(build).CreateSite(fixture);
        }

        #endregion

        #region Page

        public static IPageBuilder<T> Create<T>(this Fixture fixture, Action<T> build = null) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).Create(build);
        }

        public static IPageBuilder<T> CreateMany<T>(this Fixture fixture, int total) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).CreateMany<T>(total);
        }

        public static IPageBuilder<T> CreateMany<T>(this Fixture fixture, int total, Action<T, int> build) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).CreateMany<T>(total, build);
        }

        public static IPageBuilder<T> CreateMany<T>(this Fixture fixture, int total, Action<T> build) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).CreateMany<T>(total, build);
        }

        public static IPageBuilder<T> CreatePath<T>(this Fixture fixture, int depth) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).CreatePath(depth);
        }

        public static IPageBuilder<T> CreatePath<T>(this Fixture fixture, int depth, Action<T> build) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).CreatePath(depth, build);
        }

        public static IPageBuilder<T> CreatePath<T>(this Fixture fixture, int depth, Action<T, int> build = null) where T : PageData
        {
            fixture.Reset();
            return new PageBuilder<T>(fixture).CreatePath(depth, build);
        }

        public static IPageBuilder<T> Update<T>(this Fixture fixture, Action<T> build) where T : PageData
        {
            var contents = fixture.Contents.OfType<T>().ToList();
            fixture.Add(contents);

            return new PageBuilder<T>(fixture).Update(build);
        }

        public static IPageBuilder<T> Get<T>(this Fixture fixture) where T : PageData
        {
            var contents = fixture.Contents.OfType<T>().ToList();
            fixture.Add(contents);

            return new PageBuilder<T>(fixture);
        }

        #endregion

        #region Block

        public static IBlockBuilder<TBlockType> CreateBlock<TBlockType>(this Fixture fixture, Action<TBlockType> build = null) where TBlockType : BlockData
        {
            fixture.Reset();
            return new BlockBuilder<TBlockType>(fixture).CreateBlock(build);
        }

        #endregion

        #region Media

        public static IMediaBuilder<TMediaType> Upload<TMediaType>(this Fixture fixture, string file, Action<TMediaType> build = null) where TMediaType : MediaData
        {
            fixture.Reset();
            return new MediaBuilder<TMediaType>(fixture).Upload(file, build);
        }

        #endregion

    }
}
