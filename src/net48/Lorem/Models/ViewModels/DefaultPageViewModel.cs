using Lorem.Models.Pages;

namespace Lorem.Models.ViewModels
{
    public class DefaultPageViewModel<T> where T : SitePage
    {
        public DefaultPageViewModel(T currentPage)
        {
            CurrentPage = currentPage;
        }

        public T CurrentPage { get; set; }
    }
}