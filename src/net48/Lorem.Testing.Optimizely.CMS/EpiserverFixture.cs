using EPiServer;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Lorem.Testing.Optimizely.CMS.Commands;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Lorem.Testing.Optimizely.CMS
{
    public abstract class EpiserverFixture
    {
        public EpiserverEngine Engine { get; protected set; }

        private Dictionary<string, object> _register = new Dictionary<string, object>();
        private Dictionary<Type, List<Action<object>>> _builders = new Dictionary<Type, List<Action<object>>>();

        public SiteDefinition Site { get; set; }

        public List<CultureInfo> Cultures { get; set; } = new List<CultureInfo>();

        public List<IContent> Contents { get; set; } = new List<IContent>();

        public List<IContent> Latest { get; set; } = new List<IContent>();

        public List<IContent> GetLatestAs(CultureInfo culture)
        {
            var repository = GetInstance<IContentLoader>();

            List<IContent> latest = new List<IContent>();

            foreach (var content in Latest)
            {
                latest.Add(
                    repository.Get<IContent>(content.ContentLink.ToReferenceWithoutVersion(), culture)
                );
            }

            return latest;
        }

        public void RegisterBuilder<T>(Action<T> build) where T : IContentData
        {
            if (!_builders.ContainsKey(typeof(T)))
            {
                _builders.Add(typeof(T), new List<Action<object>>());
            }

            _builders[typeof(T)].Add(p=> build.Invoke((T)p));
        }

        public IEnumerable<Action<object>> GetBuilders<T>() where T : IContentData
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
            var exists = Contents.FirstOrDefault(c => c.ContentLink.CompareToIgnoreWorkID(content.ContentLink));

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

        public void CreateUser(string username, string password, string email, params string[] roles)
        {
            var command = new CreateUser(username, password, email, roles);
            command.Execute();
        }
    }
}
