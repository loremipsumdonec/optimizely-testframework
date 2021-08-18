using EPiServer;
using EPiServer.Core;
using Lorem.Test.Framework.Optimizely.CMS.Commands;
using Lorem.Test.Framework.Optimizely.CMS.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS.Builders
{
    public class BlockBuilder<T>
        : FixtureBuilder<T>, IBlockBuilder<T> where T: BlockData
    {
        private List<BlockData> _blocks = new List<BlockData>();

        public BlockBuilder(Fixture fixture)
            : base(fixture)
        {
        }

        public BlockBuilder(Fixture fixture, IEnumerable<BlockData> blocks)
            : base(fixture, blocks.Select(b=> (IContent)b))
        {
        }

        public IBlockBuilder<TBlockType> CreateBlock<TBlockType>(Action<TBlockType> build = null)
            where TBlockType : BlockData
        {
            CreateBlock(GetParent(), build:build);

            return new BlockBuilder<TBlockType>(Fixture, _blocks);
        }

        public IBlockBuilder<T> Update<TPageType>(Action<TPageType, T> build)
            where TPageType : PageData
        {
            var block = Fixture.Latest.OfType<T>().FirstOrDefault();

            TPageType page;

            new PageBuilder<TPageType>(Fixture)
                .Update<TPageType>(p => {
                    build.Invoke(p, block);
                    page = p;
                });

            Fixture.Latest.Clear();
            Add(block);

            return new BlockBuilder<T>(Fixture, _blocks);
        }

        private void CreateBlock<TBlockType>(ContentReference parent, CultureInfo culture = null, Action < TBlockType> build = null)
            where TBlockType : BlockData
        {
            TBlockType block = default;

            if (Fixture.Cultures.Count == 0)
            {
                throw new InvalidOperationException("Need atleast one culture");
            }

            List<CultureInfo> cultures = new List<CultureInfo>(Fixture.Cultures);

            if (culture != null)
            {
                cultures.Clear();
                cultures.Add(culture);
            }

            foreach(var c in cultures)
            {
                if(block is null)
                {
                    var command = new CreateBlock(
                        Fixture.GetContentType(typeof(TBlockType)),
                        parent,
                        IpsumGenerator.Generate(1, 3, false)
                    );

                    command.Culture = c;
                    command.Build = CreateBuild(build);

                    block = (TBlockType)command.Execute();
                    Add(block);

                    continue;
                }

                if (build == null)
                {
                    Update(block, c, null);
                    continue;
                }

                Update((TBlockType)(BlockData)block, c, p => build.Invoke((TBlockType)(BlockData)p));
            }
        }

        private ContentReference GetParent()
        {
            ContentReference parent = ContentReference.GlobalBlockFolder;

            if(Fixture.Site != null)
            {
                parent = Fixture.Site.SiteAssetsRoot;
            }

            if(Fixture.Latest.Count == 1) 
            {
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

        private void Update(BlockData block, CultureInfo culture, Action<object> build = null)
        {
            var command = new UpdateContent((IContent)block)
            {
                Culture = culture,
                Build = build
            };

            BlockData content = (BlockData)command.Execute();

            Add(content);
        }

        private void Add(BlockData block)
        {
            _blocks = _blocks.Where(b => !b.GetContentGuid().Equals(block.GetContentGuid())).ToList();
            _blocks.Add(block);
        }
    }
}
