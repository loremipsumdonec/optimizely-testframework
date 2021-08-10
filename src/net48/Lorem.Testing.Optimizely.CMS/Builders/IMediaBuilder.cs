using EPiServer.Core;
using System;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public interface IMediaBuilder<T>
        : IFixtureBuilder<T>  where T : MediaData
    {
        IMediaBuilder<TMediaType> Upload<TMediaType>(string file, Action<TMediaType> build = null) where TMediaType : MediaData;
    }
}
