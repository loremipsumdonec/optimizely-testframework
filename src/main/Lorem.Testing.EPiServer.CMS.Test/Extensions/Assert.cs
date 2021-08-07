using EPiServer.Core;
using EPiServer.ServiceLocation;
using System.Collections.Generic;

namespace Xunit
{
    public partial class Assert
    {
        public static void IsPublished(IEnumerable<IContent> contents)
        {
            foreach(var content in contents) 
            {
                IsPublished(content);
            }
        }

        public static void IsPublished(IContent content)
        {
            var repository = ServiceLocator.Current.GetInstance<IPublishedStateAssessor>();
            True(repository.IsPublished(content, PagePublishedStatus.Published), $"Content {content.Name} with type {content.GetType()} is not published");
        }
    }
}
