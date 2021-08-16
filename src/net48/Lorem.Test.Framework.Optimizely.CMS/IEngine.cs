using EPiServer.DataAbstraction;
using System;

namespace Lorem.Test.Framework.Optimizely.CMS
{
    public interface IEngine
    {
        void Start();

        void Stop();

        ContentType GetContentType(Type type);
    }
}
