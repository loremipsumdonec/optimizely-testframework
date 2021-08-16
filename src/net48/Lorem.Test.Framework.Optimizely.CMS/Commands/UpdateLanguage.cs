using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using System.Globalization;

namespace Lorem.Test.Framework.Optimizely.CMS.Commands
{
    public class UpdateLanguage
    {
        private readonly ILanguageBranchRepository _repository;

        public UpdateLanguage(CultureInfo culture, bool enabled)
            : this(culture, enabled, ServiceLocator.Current.GetInstance<ILanguageBranchRepository>())
        {
        }

        public UpdateLanguage(CultureInfo culture, bool enabled, ILanguageBranchRepository repository)
        {
            Culture = culture;
            Enabled = enabled;
            _repository = repository;
        }

        public bool Enabled { get; set; }

        public CultureInfo Culture { get; set; }

        public void Execute() 
        {
            var branch = _repository.Load(Culture);

            if (branch != null)
            {
                branch = branch.CreateWritableClone();

                branch.Enabled = Enabled;
                _repository.Save(branch);
            }
            else
            {
                branch = new LanguageBranch(Culture);
                branch.Enabled = Enabled;
                _repository.Save(branch);
            }
        }
    }
}
