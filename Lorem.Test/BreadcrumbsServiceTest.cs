using Lorem.Features.BreadCrumbs.Services;
using Lorem.Models.Pages;
using Lorem.Test.Services;
using System.Linq;
using Xunit;

namespace Lorem.Test
{
    [Collection("Episerver")]
    public class BreadCrumbsServiceTest
    {
        public BreadCrumbsServiceTest(EpiserverFixture fixture)
        {
            Fixture = fixture;
        }

        public EpiserverFixture Fixture { get; set; }

        [Fact]
        public void GetBreadCrumbs_AllPagesVisibleInBreadCrumb_HasExpectedCount()
        {
            int depth = 3;

            var page = Fixture.CreatePath<ArticlePage>(depth, build: p => p.VisibleInBreadCrumb = true);

            var service = Fixture.GetInstance<IBreadCrumbService>();
            var breadcrumbs = service.GetBreadCrumbs(page);

            Assert.Equal(depth, breadcrumbs.Count);
        }

        [Fact]
        public void GetBreadCrumbs_OnePageInPathNotVisibleInBreadCrumb_HasExpectedCount()
        {
            var pages = Fixture.Create<StartPage>(p => p.VisibleInBreadCrumb = true)
                .Create<ArticlePage>(p => p.VisibleInBreadCrumb = false)
                .Create(p => p.VisibleInBreadCrumb = true);

            var service = Fixture.GetInstance<IBreadCrumbService>();
            var breadcrumbs = service.GetBreadCrumbs(pages.Last());

            Assert.Equal(pages.Count() - 1, breadcrumbs.Count);
        }

        [Fact]
        public void GetBreadCrumbs_PageVisibleInBreadCrumb_Single()
        {
            var page = Fixture.Create<ArticlePage>(build: p => p.VisibleInBreadCrumb = true);

            var service = Fixture.GetInstance<IBreadCrumbService>();
            var breadcrumbs = service.GetBreadCrumbs(page);

            Assert.Single(breadcrumbs);
        }

        [Fact]
        public void GetBreadCrumbs_PageNotVisibleInBreadCrumb_Empty()
        {
            var page = Fixture.Create<ArticlePage>(build: p => p.VisibleInBreadCrumb = false);

            var service = Fixture.GetInstance<IBreadCrumbService>();
            var breadcrumbs = service.GetBreadCrumbs(page);

            Assert.Empty(breadcrumbs);
        }

    }
}
