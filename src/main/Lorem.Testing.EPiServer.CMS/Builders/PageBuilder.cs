using EPiServer.Core;
using Lorem.Testing.EPiServer.CMS.Commands;
using Lorem.Testing.EPiServer.CMS.Utility;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS.Builders
{
    public class PageBuilder<T>
        : FixtureBuilder<T>, IPageBuilder<T> where T : PageData
    {
        private List<PageData> _pages = new List<PageData>();

        public PageBuilder(EpiserverFixture fixture)
            : base(fixture)
        {
        }

        public PageBuilder(EpiserverFixture fixture, IEnumerable<PageData> pages)
            : base(fixture, pages)
        {
        }

        public IPageBuilder<T> Create(Action<T> build = null)
        {
            return Create<T>(build);
        }

        public IPageBuilder<T> Update(Action<T> build)
        {
            foreach(var content in Fixture.Latest)
            {
                var command = new UpdateContent(content);

                if (build != null)
                {
                    command.Build = p => build.Invoke((T)p);
                }

                command.Execute();
            }

            return this;
        }

        public IPageBuilder<T> Update<TPageType>(Action<TPageType, IEnumerable<T>> build) where TPageType : PageData
        {
            foreach(var content in Fixture.Contents
                .Where(c => c is TPageType)
                .Select(c => (TPageType)c))
            {
                foreach(var culture in Fixture.Cultures)
                {
                    var latest = Fixture.GetLatestAs(culture);

                    Update(
                        content,
                        culture,
                        p => build.Invoke((TPageType)p, Fixture.GetLatestAs(culture).Select(c => (T)c))
                    );
                }
            }

            return new PageBuilder<T>(Fixture);
        }

        private void Update(PageData page, CultureInfo culture, Action<object> build = null)
        {
            var command = new UpdateContent(page)
            {
                Culture = culture,
                Build = build
            };

            PageData content = (PageData)command.Execute();

            Add(content);
        }

        public IPageBuilder<T> Update(T page)
        {
            var command = new UpdateContent(page);
            command.Execute();

            return this;
        }

        public IPageBuilder<TPageType> Create<TPageType>(Action<TPageType> build = null) 
            where TPageType : PageData
        {
            Create(GetParent(), build: build);

            return new PageBuilder<TPageType>(Fixture, _pages);
        }

        public IPageBuilder<T> CreateMany(int total, Action<T, int> build = null)
        {
            return CreateMany<T>(total, build);
        }

        public IPageBuilder<TPageType> CreateMany<TPageType>(int total, Action<TPageType, int> build = null) 
            where TPageType : PageData
        {
            var parent = GetParent();

            for (int index = 0; index < total; index++)
            {
                if (build == null)
                {
                    Create<TPageType>(parent, null);
                    continue;
                }

                Create<TPageType>(parent, build: p => build.Invoke(p, index));
            }

            return new PageBuilder<TPageType>(Fixture, _pages);
        }

        public IPageBuilder<T> CreatePath(int depth, Action<T> build = null)
        {
            return CreatePath<T>(depth, build);
        }

        public IPageBuilder<TPageType> CreatePath<TPageType>(int depth, Action<TPageType> build = null)
            where TPageType : PageData
        {
            var parent = GetParent();

            for (int index = 0; index < depth; index++)
            {
                if(_pages.Count > 0)
                {
                    parent = _pages.Last().ContentLink;
                }

                Create(parent, build:build);
            }

            return new PageBuilder<TPageType>(Fixture, _pages);
        }

        public IPageBuilder<T> Upload<TMediaType>(IEnumerable<string> file, Action<TMediaType, T> build = null) where TMediaType : MediaData
        {
            foreach (T content in Fixture.Latest)
            {
                var command = new UploadFile(
                    IpsumGenerator.Generate(3, false).Replace(" ", "_"),
                    file.PickRandom(),
                    Fixture.GetContentType(typeof(TMediaType)),
                    content.ContentLink
                );

                if (build != null)
                {
                    command.Build = f => build.Invoke((TMediaType)f, content);
                }

                command.Execute();

                Update(content);
            }

            return new PageBuilder<T>(Fixture);
        }

        private ContentReference GetParent()
        {
            ContentReference parent = ContentReference.RootPage;

            var page = Fixture.Latest.LastOrDefault(p => p is PageData);

            if (page != null)
            {
                parent = page.ContentLink;
            }

            return parent;
        }

        private void Create<TPageType>(ContentReference parent, CultureInfo culture = null, Action<TPageType> build = null)
            where TPageType : PageData
        {
            TPageType page = default;

            if(Fixture.Cultures.Count == 0)
            {
                throw new InvalidOperationException("Need atleast one culture");
            }

            List<CultureInfo> cultures = new List<CultureInfo>(Fixture.Cultures);

            if(culture != null)
            {
                cultures.Clear();
                cultures.Add(culture);
            }

            foreach(var c in cultures)
            {
                if(page is null)
                {
                    var command = new CreatePage(
                        Fixture.GetContentType(typeof(TPageType)),
                        parent,
                        IpsumGenerator.Generate(1, 3, false)
                    );

                    command.Culture = c;

                    if (build != null)
                    {
                        command.Build = p => build.Invoke((TPageType)p);
                    }

                    page = (TPageType)command.Execute();
                    Add(page);

                    continue;
                }

                if (build == null)
                {
                    Update(page, c, null);
                    continue;
                }

                Update((T)(PageData)page, c, p => build.Invoke((TPageType)(PageData)p));
            }
        }

        private void Add(PageData page) 
        {
            _pages = _pages.Where(p => !p.ContentGuid.Equals(page.ContentGuid)).ToList();
            _pages.Add(page);
        }
    }
}
