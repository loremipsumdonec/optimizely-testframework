﻿using EPiServer.Core;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public interface IContentBuilder<T>
        : IFixtureBuilder<T> where T : IContentData
    {
        IContentBuilder<T> Publish();

        IContentBuilder<T> Expire();

        IContentBuilder<T> Delete();

        void ForceDelete();

        IContentBuilder<T> Move(ContentReference destination);
    }
}
