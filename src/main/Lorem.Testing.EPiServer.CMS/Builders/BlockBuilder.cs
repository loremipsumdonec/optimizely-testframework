using EPiServer.Core;
using Lorem.Testing.EPiServer.CMS.Commands;
using Lorem.Testing.EPiServer.CMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public class BlockBuilder<T>
        : FixtureBuilder<T>, IBlockBuilder<T> where T: BlockData
    {
        private readonly List<BlockData> _blocks = new List<BlockData>();

        public BlockBuilder(EpiserverFixture fixture)
            : base(fixture)
        {
        }

        public BlockBuilder(EpiserverFixture fixture, IEnumerable<BlockData> blocks)
            : base(fixture, blocks.Select(b=> (IContent)b))
        {
        }

        public IBlockBuilder<TBlockType> CreateBlock<TBlockType>(Action<TBlockType> build = null)
            where TBlockType : BlockData
        {
            CreateBlock(GetParent(), build);

            return new BlockBuilder<TBlockType>(Fixture, _blocks);
        }

        private void CreateBlock<TBlockType>(ContentReference parent, Action<TBlockType> build = null)
            where TBlockType : BlockData
        {
            var command = new CreateBlock(
                Fixture.GetContentType(typeof(TBlockType)),
                parent,
                IpsumGenerator.Generate(1, 3, false)
            );

            if (build != null)
            {
                command.Build = p => build.Invoke((TBlockType)p);
            }

            _blocks.Add(command.Execute());
        }

        private ContentReference GetParent()
        {
            ContentReference parent = Fixture.Site.SiteAssetsRoot;

            var page = Fixture.Latest.LastOrDefault(p => p is PageData);

            if (page != null)
            {
                parent = page.ContentLink;
            }

            return parent;
        }
    }
}
