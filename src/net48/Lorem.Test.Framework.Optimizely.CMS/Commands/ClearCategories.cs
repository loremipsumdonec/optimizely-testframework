using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;

namespace Lorem.Test.Framework.Optimizely.CMS.Commands
{
    public class ClearCategories
        : IClearCommand
    {
        private readonly CategoryRepository _repository;

        public ClearCategories()
            : this(ServiceLocator.Current.GetInstance<CategoryRepository>())
        {
        }

        public ClearCategories(CategoryRepository repository)
        {
            _repository = repository;
        }

        public void Clear()
        {
            var root = _repository.GetRoot();

            foreach (var category in root.Categories)
            {
                _repository.Delete(category);
            }
        }
    }
}
