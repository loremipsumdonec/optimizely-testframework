using EPiServer.ServiceLocation;
using EPiServer.Web;

namespace Lorem.Testing.EPiServer.CMS.Commands
{
    public class ClearSites
        : IClearCommand
    {
        private readonly ISiteDefinitionRepository _repository;

        public ClearSites()
            : this(ServiceLocator.Current.GetInstance<ISiteDefinitionRepository>())
        {
        }

        public ClearSites(ISiteDefinitionRepository repository)
        {
            _repository = repository;
        }

        public void Clear()
        {
            foreach (var site in _repository.List())
            {
                _repository.Delete(site.Id);
            }
        }
    }
}
