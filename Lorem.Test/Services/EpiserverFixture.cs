using EPiServer;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;
using EPiServer.DataAbstraction;
using EPiServer.DataAccess;
using EPiServer.Framework.Configuration;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;

namespace Lorem.Test.Services
{
    public class EpiserverFixture
    {
        private readonly EpiserverEngineThirdIteration _engine;
        private readonly List<ContentType> _contentTypes;

        public EpiserverFixture()
        {
            _engine = EpiserverEngineThirdIteration.GetInstance();
            _engine.Start();

            _contentTypes = GetContentTypes();
            CreateDefaultUser();
        }

        private void CreateDefaultUser()
        {
            CreateUser("Administrator", "Administrator", "loremipsumdonec@supersecretpassword.io");
        }

        public ContentType GetContentType(Type type)
        {
            var contentType = _contentTypes.FirstOrDefault(c => c.ModelType.Equals(type));

            if(contentType == null)
            {
                throw new ArgumentException($"could not find a valid content type for type {type}");
            }

            return contentType;
        }

        private List<ContentType> GetContentTypes()
        {
            var repository = GetInstance<IContentTypeRepository>();
            return new List<ContentType>(repository.List());
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

        public PageBuilder<T> Create<T>(Action<T> build = null)
        {
            return new PageBuilder<T>(this).Create(build);
        }

        public PageBuilder<T> CreatePath<T>(int depth, Action<T> build = null) where T: PageData
        {
            return new PageBuilder<T>(this).CreatePath(depth, build);
        }

        public void Dispose()
        {
        }
    }

    public class PageBuilder<T>
        : IEnumerable<PageData>
    {
        private readonly EpiserverFixture _fixture;
        private IEnumerable<PageData> _pages;

        public PageBuilder(EpiserverFixture fixture)
        {
            _fixture = fixture;
            _pages = new List<PageData>();
        }

        public PageBuilder(EpiserverFixture fixture, IEnumerable<PageData> pages)
        {
            _fixture = fixture;
            _pages = pages;
        }

        public PageBuilder<T> Create(Action<T> build = null)
        {
            return Create<T>(build);
        }

        public PageBuilder<TPageType> Create<TPageType>(Action<TPageType> build = null)
        {
            ContentReference parent = ContentReference.RootPage;

            if (_pages.Count() > 0)
            {
                parent = _pages.Last().ContentLink;
            }

            Create(parent, build);

            return new PageBuilder<TPageType>(_fixture, _pages);
        }

        public PageBuilder<T> CreateMany(int total, Action<T, int> build = null)
        {
            return CreateMany<T>(total, build);
        }

        public PageBuilder<TPageType> CreateMany<TPageType>(int total, Action<TPageType, int> build = null) 
        {
            ContentReference parent = ContentReference.RootPage;

            if (_pages.Count() > 0)
            {
                parent = _pages.Last().ContentLink;
            }

            for (int index = 0; index < total; index++)
            {
                if(build == null)
                {
                    Create<TPageType>(parent, null);
                    continue;
                }

                Create<TPageType>(parent, p => build.Invoke(p, index));
            }

            return new PageBuilder<TPageType>(_fixture, _pages);
        }

        public PageBuilder<T> CreatePath(int depth, Action<T> build = null)
        {
            return CreatePath<T>(depth, build);
        }

        public PageBuilder<TPageType> CreatePath<TPageType>(int depth, Action<TPageType> build = null)
        {
            for (int index = 0; index < depth; index++)
            {
                Create<TPageType>(build);
            }

            return new PageBuilder<TPageType>(_fixture, _pages);
        }

        private void Create<TPageType>(ContentReference parent, Action<TPageType> build = null)
        {
            var command = new CreatePage(
                _fixture.GetContentType(typeof(TPageType)),
                parent
            );

            if (build != null)
            {
                command.Build = p => build.Invoke((TPageType)p);
            }

            Add(command.Execute());
        }

        private void Add(PageData pageData)
        {
            var pages = new List<PageData>(_pages);
            pages.Add(pageData);

            _pages = pages;
        }

        public IEnumerator<PageData> GetEnumerator()
        {
            return _pages.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _pages.GetEnumerator();
        }
    
        static public implicit operator PageData(PageBuilder<T> builder)
        {
            return builder.Last();
        }
    }

    public class CreatePage
    {
        private readonly IContentRepository _repository;

        public CreatePage(ContentType contentType, ContentReference parent)
            : this(contentType, parent, ServiceLocator.Current.GetInstance<IContentRepository>())
        {
        }

        public CreatePage(ContentType contentType, ContentReference parent, IContentRepository repository)
        {
            ContentType = contentType;
            Parent = parent;
            _repository = repository;
        }

        public string Name { get; set; }

        public ContentType ContentType { get; set; }

        public ContentReference Parent { get; set; }

        public CultureInfo Language { get; set; }

        public SaveAction SaveAction { get; set; } = SaveAction.Publish | SaveAction.ForceCurrentVersion;

        public Action<object> Build { get; set; }

        public PageData Execute()
        {
            var page = GetDefault();
            page.Name = GetName();

            if(Build != null)
            {
                Save(page);
                Build(page);
            }

            return Save(page);
        }

        private PageData GetDefault()
        {
            return _repository.GetDefault<PageData>(
                    Parent,
                    ContentType.ID,
                    Language);
        }

        private string GetName()
        {
            if (string.IsNullOrEmpty(Name))
            {
                return Guid.NewGuid().ToString("N"); //IpsumGenerator.Generate(1, 2, false);
            }

            return Name;
        }

        private PageData Save(IContent page)
        {
            var contentReference = _repository.Save(
                    page,
                    SaveAction,
                    AccessLevel.NoAccess
                );

            return _repository.Get<PageData>(contentReference).CreateWritableClone();
        }
    }

    public class CreateUser
    {
        public CreateUser(string username, string password, string email)
        {
            Username = username;
            Password = password;
            Email = email;
            Roles = new List<string>();
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
    
        public void Add(params string[] roles)
        {
            Roles.AddRange(roles);
        }

        public void Execute() 
        {
            var connectionStringsSection = ConfigurationSource.Instance.Get<ConnectionStringsSection>("connectionStrings");
            string connectionString = connectionStringsSection.ConnectionStrings["EPiServerDB"].ConnectionString;

            using (UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(
                new ApplicationDbContext<ApplicationUser>(connectionString)))
            {
                if (!store.Users.Any(x => x.UserName == Username))
                {
                    ApplicationUser user = Create(Username, Password, Email, store);
                    AddRolesToUser(new string[] { "WebAdmins", "WebEditors", "Administrators" }, user, store, connectionString);

                    store.UpdateAsync(user)
                        .GetAwaiter()
                        .GetResult();
                }
            }
        }

        private ApplicationUser Create(
            string username,
            string password,
            string email,
            UserStore<ApplicationUser> store)
        {
            IPasswordHasher hasher = new PasswordHasher();
            string passwordHash = hasher.HashPassword(password);

            ApplicationUser applicationUser = new ApplicationUser
            {
                Email = email,
                EmailConfirmed = true,
                LockoutEnabled = true,
                IsApproved = true,
                UserName = username,
                PasswordHash = passwordHash,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            store.CreateAsync(applicationUser)
                .GetAwaiter()
                .GetResult();

            return store.FindByNameAsync(username)
                .GetAwaiter()
                .GetResult();
        }

        private void AddRolesToUser(
            IEnumerable<string> roles,
            ApplicationUser user,
            UserStore<ApplicationUser> store,
            string connectionString)
        {
            IUserRoleStore<ApplicationUser, string> userRoleStore = store;
            using (var roleStore = new RoleStore<IdentityRole>(new ApplicationDbContext<ApplicationUser>(connectionString)))
            {
                IList<string> userRoles = userRoleStore.GetRolesAsync(user)
                    .GetAwaiter().GetResult();

                foreach (string roleName in roles)
                {
                    if (roleStore.FindByNameAsync(roleName).GetAwaiter().GetResult() == null)
                    {
                        roleStore.CreateAsync(new IdentityRole { Name = roleName })
                            .GetAwaiter()
                            .GetResult();
                    }

                    if (!userRoles.Contains(roleName))
                    {
                        userRoleStore.AddToRoleAsync(user, roleName)
                            .GetAwaiter()
                            .GetResult();
                    }
                }
            }
        }
    }
}
