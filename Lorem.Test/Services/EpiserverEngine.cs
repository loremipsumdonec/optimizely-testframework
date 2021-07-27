using EPiServer.Framework;
using EPiServer.Framework.Configuration;
using EPiServer.Framework.Initialization;
using EPiServer.Web.Hosting;
using Lorem.Test.TestFrameworks;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Lorem.Test.Services
{
    public class EpiserverEngine
    {
        private static EpiserverEngine _singelton;
        private string _webConfig;
        private InitializationEngine _engine;
        private List<Assembly> _assemblies;
        private readonly List<IInitializableModule> _modules = new List<IInitializableModule>();
        private readonly List<IEpiserverTestFramework> _testFrameworks = new List<IEpiserverTestFramework>();
        private bool _started;

        public static EpiserverEngine GetInstance() 
        {
            if(_singelton == null)
            {
                _singelton = new EpiserverEngine();
                _singelton.Add(new CmsTestFrameworks());
            }

            return _singelton;
        }

        private EpiserverEngine() 
        { 
        }

        public void Add(IEpiserverTestFramework testFramework)
        {
            _testFrameworks.Add(testFramework);
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
            foreach(var testFramework in _testFrameworks) 
            {
                testFramework.Reset();
            }
        }

        public void Stop()
        {
            Uninitialize();
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
            List<string> names = new List<string>()
            {
                "EPiServer",
                CurrentAssemblyName()
            };

            _assemblies = new AssemblyList(true).AllowedAssemblies.Where(
                    a => names.Contains(a.GetName().Name.Split('.')[0])
                ).ToList();
        }

        private void LoadInitializationEngine()
        {
            _engine = new InitializationEngine((List<IInitializableModule>)null, HostType.TestFramework, _assemblies);
        }

        private void BeforeInitialize()
        {
            foreach(var testFramework in _testFrameworks)
            {
                testFramework.BeforeInitialize();
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
            foreach (var testFramework in _testFrameworks)
            {
                testFramework.AfterInitialize();
            }
        }

        private string CurrentAssemblyName()
        {
            return typeof(EpiserverEngine).Assembly.GetName().Name.Split('.')[0];
        }
    }
}
