using EPiServer.Core;
using EPiServer.DataAnnotations;

namespace Lorem.Models.Pages
{
    [ContentType(DisplayName = "Start page", GUID = "AF8153A7-4F25-4255-B91D-43A7F04A29D2")]
    public class StartPage
        : SitePage
    {
        [CultureSpecific]
        public virtual ContentArea ContentArea {get; set;}

        public void Add(IContent content)
        {
            if(ContentArea == null)
            {
                ContentArea = new ContentArea();
            }

            ContentArea.Items.Add(
                new ContentAreaItem() { ContentLink = content.ContentLink }
            );
        }
    }
}
