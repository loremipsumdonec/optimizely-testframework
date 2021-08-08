using EPiServer.Core;
using System;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public interface IMediaBuilder
        : IFixtureBuilder
    {
    }

    public interface IMediaBuilder<T>
        : IFixtureBuilder<T> , IMediaBuilder where T : MediaData
    {
        IMediaBuilder<TMediaType> Upload<TMediaType>(string file, Action<TMediaType> build = null) where TMediaType : MediaData;
    }
}
