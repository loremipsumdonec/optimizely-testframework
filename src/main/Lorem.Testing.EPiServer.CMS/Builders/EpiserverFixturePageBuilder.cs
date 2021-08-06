using EPiServer.Core;
using System;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public static class EpiserverFixturePageBuilder
    {
        public static ISiteBuilder<T> CreateSite<T>(this EpiserverFixture fixture, Action<T> build = null) where T : PageData
            => new PageBuilder<T>(fixture).Create<T>(build).CreateSite(fixture);

        public static IPageBuilder<T> Create<T>(this EpiserverFixture fixture, Action<T> build = null) where T : PageData
            => new PageBuilder<T>(fixture).Create(build);

        public static IPageBuilder<T> Create<T>(this EpiserverFixture fixture, CultureInfo[] cultures, Action<T> build = null) where T : PageData
            => new PageBuilder<T>(fixture).Create(cultures, build);

        public static IPageBuilder<T> CreateMany<T>(this EpiserverFixture fixture, int total, Action<T, int> build = null) where T : PageData
            => new PageBuilder<T>(fixture).CreateMany(total, build);

        public static IPageBuilder<T> CreatePath<T>(this EpiserverFixture fixture, int depth, Action<T> build = null) where T : PageData
            => new PageBuilder<T>(fixture).CreatePath(depth, build);

        public static IPageBuilder<T> Update<T>(this EpiserverFixture fixture, Action<T> build) where T : PageData
        {
            var contents = fixture.Contents.Where(c => c is T).Select(c => (T)c).ToList();
            fixture.Add(contents);

            return new PageBuilder<T>(fixture).Update(build);
        }

        public static IPageBuilder<T> Get<T>(this EpiserverFixture fixture) where T : PageData
        {
            var contents = fixture.Contents.Where(c => c is T).Select(c => (T)c).ToList();
            fixture.Add(contents);

            return new PageBuilder<T>(fixture);
        }

        public static IBlockBuilder<TBlockType> CreateBlock<TBlockType, TPageType>(this EpiserverFixture fixture, Action<TBlockType, TPageType> build)
            where TBlockType : BlockData where TPageType : PageData
        {
            var page = fixture.Get<TPageType>().Last();
            var blockBuilder = fixture.Get<TPageType>().CreateBlock<TBlockType>((b) => build.Invoke(b, page));
            var latest = fixture.Latest.Last();

            var pageBuilder = new PageBuilder<TPageType>(fixture).Update(page);

            fixture.Add(latest);

            return blockBuilder;
        }
    }
}
