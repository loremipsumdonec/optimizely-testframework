using EPiServer.DataAbstraction;
using EPiServer.Framework;
using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Hosting;
using Lorem.Testing.EPiServer.CMS.Commands;
using Lorem.Testing.EPiServer.CMS.TestFrameworks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lorem.Testing.EPiServer.CMS
{
    public class EpiserverEngine
        : IEpiserverEngine
    {
        private string _webConfig;
        private InitializationEngine _engine;
        private List<Assembly> _assemblies;
        private readonly List<IEpiserverTestFramework> _frameworks = new List<IEpiserverTestFramework>();
        private bool _started;
        private List<ContentType> _contentTypes;

        public EpiserverEngine()
        {
        }

        public EpiserverEngine(params IEpiserverTestFramework[] frameworks)
        {
            _frameworks = new List<IEpiserverTestFramework>(frameworks);
        }

        public void Add(IEpiserverTestFramework testFramework)
        {
            _frameworks.Add(testFramework);
        }

        public void Start()
        {
            if(_started)
            {
                Reset();
                return;
            }

            LoadWebConfig();
            LoadHostingEnvironment();
            LoadConfigurationSource();
            LoadAssemblies();
            LoadInitializationEngine();

            BeforeInitialize();
            Initialize();
            AfterInitialize();

            _started = true;
        }

        private void Reset()
        {
            List<IClearCommand> clearCommands = new List<IClearCommand>();

            foreach (var testFramework in _frameworks)
            {
                clearCommands.AddRange(
                    testFramework.Reset()
                );
            }

            foreach (var command in clearCommands)
            {
                command.Clear();
            }
        }

        public void Stop()
        {
            Uninitialize();
        }

        public ContentType GetContentType(Type type)
        {
            if (_contentTypes == null)
            {
                var repository = ServiceLocator.Current.GetInstance<IContentTypeRepository>();
                _contentTypes = new List<ContentType>(repository.List());
            }

            var contentType = _contentTypes.FirstOrDefault(c => c.ModelType != null && c.ModelType.Equals(type));

            if (contentType == null)
            {
                throw new ArgumentException($"could not find a valid content type for type {type}");
            }

            return contentType;
        }

        private void LoadWebConfig()
        {
            _webConfig = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Web.config");

            if (!File.Exists(_webConfig))
            {
                throw new FileNotFoundException($"Could not load Web.config at expected path {_webConfig}");
            }
        }

        private void LoadHostingEnvironment()
        {
            string physicalPath = Path.GetDirectoryName(_webConfig);
            GenericHostingEnvironment.Instance = new NoneWebContextHostingEnvironment("/", physicalPath);
        }

        private void LoadConfigurationSource()
        {
            ExeConfigurationFileMap fileMap = new ExeConfigurationFileMap()
            {
                ExeConfigFilename = _webConfig
            };

            Configuration config = ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
            ConfigurationSource.Instance = new FileConfigurationSource(config);
        }

        private void LoadAssemblies()
        {
            _assemblies = new AssemblyList(true).AllowedAssemblies.ToList();
        }

        private void LoadInitializationEngine()
        {
            _engine = new InitializationEngine(new List<IInitializableModule>(), HostType.TestFramework, _assemblies);
            _engine.ScanAssemblies();
        }

        private void BeforeInitialize()
        {
            foreach(var testFramework in _frameworks)
            {
                testFramework.BeforeInitialize(_engine);
            }
        }

        private void Initialize()
        {
            if(!_started)
            {
                _engine.Initialize();
            }
        }

        private void Uninitialize()
        {
            _engine.Uninitialize();
        }

        private void AfterInitialize()
        {
            foreach (var testFramework in _frameworks)
            {
                testFramework.AfterInitialize(_engine);
            }
        }
    }
}
