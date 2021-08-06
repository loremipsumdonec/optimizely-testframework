using Alloy.Test.Services;
using AlloyTemplates.Models.Blocks;
using AlloyTemplates.Models.Pages;
using EPiServer;
using EPiServer.Core;
using EPiServer.SpecializedProperties;
using Lorem.Testing.EPiServer.CMS.Builders;
using Lorem.Testing.EPiServer.CMS.Utility;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Xunit;

namespace Alloy.Test
{
    [Collection("Default")]
    public class ExampleApiForTestFramework
    {
        public ExampleApiForTestFramework(DefaultEpiserverEngine engine)
        {
            Fixture = new DefaultEpiserverFixture(engine);
            Resources = new DefaultResources();
        }

        public DefaultResources Resources { get; set; }

        public DefaultEpiserverFixture Fixture { get; set; }

        [Theory]
        [InlineData(1)]
        public async void CreateSite(int _)
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

            var cultures = new CultureInfo[]
            {
                 CultureInfo.GetCultureInfo("sv"),
                 CultureInfo.GetCultureInfo("en")
            };

            Fixture.Create<StartPage>(cultures);

            /*
            Fixture.CreateSite<StartPage>()
               .CreateMany<ProductPage>(5, (p, _) =>
               {
                   p.UniqueSellingPoints = sellingPoints.PickRandom(3, 7).ToList();
                   p.MetaDescription = IpsumGenerator.Generate(12, 23);
               })
               .Update<StartPage>((p, products) => {

                   p.ProductPageLinks = new LinkItemCollection();

                   foreach (var product in products)
                   {
                       p.ProductPageLinks.Add(
                           new LinkItem()
                           {
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

            var client = Fixture.Engine.CreateClient();

            var response = await client.GetAsync("/");
            response.EnsureSuccessStatusCode();
            */
        }
    }
}
