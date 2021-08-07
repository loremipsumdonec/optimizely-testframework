using EPiServer.Core;
using EPiServer.ServiceLocation;
using System.Collections.Generic;

namespace Xunit
{
    public partial class Assert
    {
        public static void IsPublished(IEnumerable<IContentData> contents)
        {
            foreach(var content in contents)
            {
                IsPublished(content);
            }
        }

        public static void IsPublished(IContentData content)
        {
            var repository = ServiceLocator.Current.GetInstance<IPublishedStateAssessor>();
            True(repository.IsPublished((IContent)content, PagePublishedStatus.Published), $"Content is not published");
        }

        public static void IsExpired(IContentData content)
        {
            var repository = ServiceLocator.Current.GetInstance<IPublishedStateAssessor>();
            True(!repository.IsPublished((IContent)content, PagePublishedStatus.Published), $"Content has not expired");
        }
    }
}
