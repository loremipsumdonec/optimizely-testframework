using Alloy.Models.Blocks;
using Alloy.Models.Media;
using Alloy.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using Alloy.Test.Services;
using Alloy.Test.Utility;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Lorem.Testing.EPiServer.CMS;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Utility;

namespace Alloy.Test
{
    [Collection("Episerver")]
    public class ExampleApiForTestFramework
    {
        public ExampleApiForTestFramework(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
            Resources = new DefaultResources();
        }

        public DefaultResources Resources { get; set; }

        public EpiserverFixture Fixture { get; set; }

        [Fact]
        public void CreateSite()
        {
            List<string> sellingPoints = new List<string>()
            {
                "Online meeting",
                "distance cooperation",
                "project calendar",
                "white board",
                "online presentations",
                "Project planning",
                "Reporting and statistics",
                "Email handling of tasks",
                "Risk calculations",
                "Direct communication to members",
            };

            Fixture.CreateSite<StartPage>()
               .CreateMany<ProductPage>(5, (p, _) =>
               {
                   p.UniqueSellingPoints = sellingPoints.PickRandom(3, 7).ToList();
                   p.MetaDescription = IpsumGenerator.Generate(12, 23);
               })
               .Upload<ImageFile>(Resources.Get("/images"), (f, p) => {
                    p.PageImage = f.ContentLink;
               })
               .Update<StartPage>((p, products) => {

                   p.ProductPageLinks = new LinkItemCollection();

                   foreach (var product in products)
                   {
                       p.ProductPageLinks.Add(
                           new LinkItem() {
                               Href = product.LinkURL,
                               Text = product.Name
                        });
                   }
               });

            Fixture.CreateBlock<JumbotronBlock, StartPage>((b, s) =>
            {
                ProductPage product = (ProductPage)Fixture.Contents.Where(p => p is ProductPage).PickRandom();

                b.Heading = product.Name;
                b.SubHeading = IpsumGenerator.Generate(15, 17, false);
                b.ButtonText = $"Read more";
                b.ButtonLink = new Url(product.LinkURL);

                s.MainContentArea = new ContentArea();

                s.MainContentArea.Items.Add(
                    new ContentAreaItem() { ContentLink = b.GetContentLink() }
                );
            });
        }
    }
}
