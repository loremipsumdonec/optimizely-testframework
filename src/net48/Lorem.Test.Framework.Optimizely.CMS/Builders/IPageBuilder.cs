using EPiServer.Core;
using System;
using System.Collections.Generic;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public interface IPageBuilder<T>
        : IFixtureBuilder<T> where T : PageData
    {
        IPageBuilder<T> Create(Action<T> build = null);

        IPageBuilder<T> Update(Action<T> build);

        IPageBuilder<T> Update<TPageType>(Action<TPageType> build) where TPageType : PageData;

        IPageBuilder<T> Update<TPageType>(Action<TPageType, IEnumerable<T>> build) where TPageType : PageData;

        IPageBuilder<T> Update(T page);

        IPageBuilder<TPageType> Create<TPageType>(Action<TPageType> build = null) where TPageType: PageData;

        IPageBuilder<T> CreateMany(int total);

        IPageBuilder<T> CreateMany(int total, Action<T, int> build);

        IPageBuilder<T> CreateMany(int total, Action<T> build);

        IPageBuilder<TPageType> CreateMany<TPageType>(int total) where TPageType : PageData;

        IPageBuilder<TPageType> CreateMany<TPageType>(int total, Action<TPageType> build) where TPageType : PageData;

        IPageBuilder<TPageType> CreateMany<TPageType>(int total, Action<TPageType, int> build) where TPageType : PageData;

        IPageBuilder<T> CreatePath(int depth);

        IPageBuilder<T> CreatePath(int depth, Action<T> build);

        IPageBuilder<T> CreatePath(int depth, Action<T, int> build);

        IPageBuilder<TPageType> CreatePath<TPageType>(int depth) where TPageType : PageData;

        IPageBuilder<TPageType> CreatePath<TPageType>(int depth, Action<TPageType> build) where TPageType : PageData;

        IPageBuilder<TPageType> CreatePath<TPageType>(int depth, Action<TPageType, int> build) where TPageType : PageData;

        IPageBuilder<T> Upload<TMediaType>(IEnumerable<string> files, Action<TMediaType, T> build) where TMediaType : MediaData;
    }
}
