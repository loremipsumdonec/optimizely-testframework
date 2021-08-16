using EPiServer;
using EPiServer.Core;
using EPiServer.Data.Entity;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Lorem.Test.Framework.Optimizely.CMS.Commands;
using Lorem.Test.Framework.Optimizely.CMS.Services;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS
{
    public abstract class Fixture
    {
        private readonly Dictionary<string, object> _register = new Dictionary<string, object>();
        private readonly Dictionary<Type, List<Action<object>>> _builders = new Dictionary<Type, List<Action<object>>>();

        protected Fixture(IEngine engine)
        {
            Engine = engine;
        }

        public IEngine Engine { get; }

        public SiteDefinition Site { get; set; }

        public List<CultureInfo> Cultures { get; set; } = new List<CultureInfo>();

        public List<IContent> Contents { get; set; } = new List<IContent>();

        public List<IContent> Latest { get; set; } = new List<IContent>();

        protected void Start()
        {
            Engine.Start();
        }

        public T Get<T>(IContent content, CultureInfo culture) where T: IContentData
        {
            var loader = GetInstance<IContentLoader>();

            T contentAsCulture = loader.Get<T>(content.ContentGuid, culture);

            if (contentAsCulture is IReadOnly readOnly)
            {
                contentAsCulture = (T)readOnly.CreateWritableClone();
            }

            return contentAsCulture;
        }

        public List<IContent> GetLatest(CultureInfo culture)
        {
            List<IContent> latest = new List<IContent>();

            foreach (var content in Latest)
            {
                latest.Add(
                    Get<IContent>(content, culture)
                );
            }

            return latest;
        }

        public void ClearBuilders<T>()
        {
            foreach (var kv in _builders)
            {
                if (kv.Key.IsAssignableFrom(typeof(T)))
                {
                    kv.Value.Clear();
                }
            }
        }

        public void RegisterBuilder<T>(Action<T> build)
        {
            if (!_builders.ContainsKey(typeof(T)))
            {
                _builders.Add(typeof(T), new List<Action<object>>());
            }

            _builders[typeof(T)].Add(p=> build.Invoke((T)p));
        }

        public IEnumerable<Action<object>> GetBuilders<T>()
        {
            List<Action<object>> builders = new List<Action<object>>();

            foreach(var kv in _builders)
            {
                if(kv.Key.IsAssignableFrom(typeof(T)))
                {
                    builders.AddRange(kv.Value);
                }
            }

            return builders;
        }

        public void Add(IEnumerable<IContent> contents)
        {
            foreach (var content in contents)
            {
                Add(content);
            }

            Latest.Clear();
            Latest.AddRange(contents);
        }

        public void Add(IContent content)
        {
            var exists = Contents.Find(c => c.ContentLink.CompareToIgnoreWorkID(content.ContentLink));

            if (exists != null)
            {
                Contents.Remove(exists);
            }

            Contents.Add(content);
        }

        public void RegisterLatest(IEnumerable<IContent> latest)
        {
            Latest = new List<IContent>(latest);
        }

        public ContentType GetContentType(Type type)
        {
            return Engine.GetContentType(type);
        }

        public T Get<T>(string key)
        {
            return (T)_register[key];
        }

        public void Register(string key, object value)
        {
            _register.Add(key, value);
        }

        public T GetInstance<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        public NestedContext ReplaceServiceWith<T>(T instance)
        {
            var locator = (ServiceLocatorDecorator)ServiceLocator.Current;
            var context = locator.Push();

            context.Container.Configure(_ => _.For(typeof(T)).Use(instance));

            return context;
        }

        public void CreateUser(string username, string password, string email, params string[] roles)
        {
            var command = new CreateUser(username, password, email, roles);
            command.Execute();
        }

        public void Reset()
        {
            Latest.Clear();

            if(Site != null)
            {
                Latest.Add(Contents.First(p => p.ContentLink.Equals(Site.StartPage, true)));
            }
        }
    }
}
