using EPiServer.Core;
using System;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public interface IMediaBuilder<T>
        where T : MediaData
    {
        IMediaBuilder<T> Upload<TMediaType>(string file, Action<TMediaType> build = null) where TMediaType : MediaData;
    }
}
