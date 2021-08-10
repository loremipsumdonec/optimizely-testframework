using EPiServer.Core;
using System;

namespace Lorem.Testing.Optimizely.CMS.Builders
{
    public interface IBlockBuilder<T>
        : IFixtureBuilder<T> where T: BlockData
    {
        IBlockBuilder<TBlockType> CreateBlock<TBlockType>(Action<TBlockType> build = null)
            where TBlockType : BlockData;
    }
}
