using EPiServer.DataAbstraction;
using System;

namespace Lorem.Testing.EPiServer.CMS
{
    public interface IEpiserverEngine
    {
        void Start();

        void Stop();

        ContentType GetContentType(Type type);
    }
}
