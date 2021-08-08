using EPiServer.DataAbstraction;
using System;

namespace Lorem.Testing.Optimizely.CMS
{
    public abstract class EpiserverFixtureWithFactory<TEntryPoint>
        : EpiserverFixture where TEntryPoint : class
    {
        public EpiserverEngine<TEntryPoint> Engine { get; protected set; }

        public override ContentType GetContentType(Type type)
        {
            return Engine.GetContentType(type);
        }
    }
}
