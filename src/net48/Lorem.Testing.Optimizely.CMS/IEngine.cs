using EPiServer.DataAbstraction;
using System;

namespace Lorem.Testing.Optimizely.CMS
{
    public interface IEngine
    {
        void Start();

        void Stop();

        ContentType GetContentType(Type type);
    }
}
