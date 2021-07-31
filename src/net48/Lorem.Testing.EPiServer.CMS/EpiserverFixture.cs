using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.ServiceLocation;
using EPiServer.Web;
using Lorem.Testing.EPiServer.CMS.Commands;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Lorem.Testing.EPiServer.CMS
{
    public abstract class EpiserverFixture
    {
        private Dictionary<string, object> _register = new Dictionary<string, object>();

        public IEpiserverEngine Engine { get; protected set; }

        public SiteDefinition Site { get; set; }

        public List<IContent> Contents { get; set; } = new List<IContent>();

        public List<IContent> Latest { get; set; } = new List<IContent>();

        public void Add(IEnumerable<IContent> contents)
        {
            foreach(var content in contents)
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

        public void CreateDefaultUser()
        {
            CreateUser("Administrator", "Administrator", "loremipsumdonec@supersecretpassword.io");
        }

        public T GetInstance<T>()
        {
            return ServiceLocator.Current.GetInstance<T>();
        }

        public void CreateUser(string username, string password, string email, params string[] roles) 
        {
            var command = new CreateUser(username, password, email);
            command.Add(roles);
            command.Execute();
        }

        public void Dispose()
        {
        }
    }
}
