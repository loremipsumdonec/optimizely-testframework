using EPiServer.Core;
using Lorem.Testing.Optimizely.CMS.Commands;
using Lorem.Testing.Optimizely.CMS.Utility;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS.Builders
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

            command.Build = CreateBuild(build);

            _blocks.Add(command.Execute());
        }

        private ContentReference GetParent()
        {
            ContentReference parent = ContentReference.GlobalBlockFolder;

            if(Fixture.Site != null)
            {
                parent = Fixture.Site.SiteAssetsRoot;
            }

            var page = Fixture.Latest.LastOrDefault(p => p is PageData);

            if (page != null)
            {
                parent = page.ContentLink;
            }

            var block = Fixture.Latest.LastOrDefault(p => p is BlockData);

            if (block != null)
            {
                parent = block.ParentLink;
            }

            return parent;
        }

        private Action<object> CreateBuild<TBlockType>(Action<TBlockType> build)
            where TBlockType : BlockData
        {
            return p =>
            {
                foreach (var builder in Fixture.GetBuilders<TBlockType>())
                {
                    builder.Invoke(p);
                }

                build?.Invoke((TBlockType)p);
            };
        }
    }
}
