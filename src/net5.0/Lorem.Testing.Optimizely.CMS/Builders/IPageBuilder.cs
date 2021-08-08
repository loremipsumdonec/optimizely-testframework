using EPiServer.Core;
using System;
using System.Collections.Generic;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public interface IPageBuilder
        : IFixtureBuilder
    {
    }

    public interface IPageBuilder<T>
        : IFixtureBuilder<T>, IPageBuilder where T : PageData
    {
        IPageBuilder<T> Create(Action<T> build = null);

        IPageBuilder<T> Update(Action<T> build);

        IPageBuilder<T> Update<TPageType>(Action<TPageType, IEnumerable<T>> build) where TPageType : PageData;

        IPageBuilder<T> Update(T page);

        IPageBuilder<TPageType> Create<TPageType>(Action<TPageType> build = null) where TPageType: PageData;

        IPageBuilder<T> CreateMany(int total, Action<T, int> build = null);

        IPageBuilder<TPageType> CreateMany<TPageType>(int total, Action<TPageType, int> build = null) where TPageType : PageData;

        IPageBuilder<T> CreatePath(int depth, Action<T> build = null);

        IPageBuilder<TPageType> CreatePath<TPageType>(int depth, Action<TPageType> build = null) where TPageType : PageData;
    }
}
