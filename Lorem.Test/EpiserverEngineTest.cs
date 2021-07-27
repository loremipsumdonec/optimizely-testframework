using EPiServer;
using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Framework.Configuration;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using Lorem.Models.Pages;
using Lorem.Test.Services;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Xunit;

namespace Lorem.Test
{
    public class EpiserverEngineTest
    {
        [Fact]
        public void StartWithEpiserverEngineFirstIteration_IContentRepositoryIsNotNull()
        {
            var engine = new EpiserverEngineFirstIteration();
            engine.Start();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();
            Assert.NotNull(repository);

            engine.Stop();
        }

        [Fact]
        public void StartWithEpiserverEngineFirstIteration_CreateStartPage_SinglePageExists()
        {
            var engine = new EpiserverEngineFirstIteration();
            engine.Start();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);

            engine.Stop();
        }

        [Fact]
        public void StartWithEpiserverEngineFirstIteration_CreateStartPageAndUser_SinglePageExists()
        {
            var engine = new EpiserverEngineFirstIteration();
            engine.Start();

            CreateUser("Administrator", "Administrator", "loremipsumdonec@supersecretpassword.io");

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);

            engine.Stop();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void StartWithEpiserverEngineFirstIteration_RunMultipleTimes_SinglePageExists(int notUsed)
        {
            var engine = new EpiserverEngineFirstIteration();
            engine.Start();

            CreateUser("Administrator", "Administrator", "loremipsumdonec@supersecretpassword.io");

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);

            engine.Stop();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void StartWithEpiserverEngineFirstIteration_RunMultipleTimesNoUser_SinglePageExists(int notUsed)
        {
            var engine = new EpiserverEngineFirstIteration();
            engine.Start();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);

            engine.Stop();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void StartWithEpiserverEngineSecondIteration_RunMultipleTimesNoUser_SinglePageExists(int notUsed)
        {
            var engine = EpiserverEngineSecondIteration.GetInstance();
            engine.Start();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void StartWithEpiserverEngineThirdIteration_RunMultipleTimesNoUser_SinglePageExists(int notUsed)
        {
            var engine = EpiserverEngineThirdIteration.GetInstance();
            engine.Start();

            CreateUser("Administrator", "Administrator", "loremipsumdonec@supersecretpassword.io");

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        public void StartWithEpiserverEngineThirdIteration_RunMultipleTimesAndCreateUser_SinglePageExists(int notUsed)
        {
            var engine = EpiserverEngineThirdIteration.GetInstance();
            engine.Start();

            CreateUser("Administrator", "Administrator", "loremipsumdonec@supersecretpassword.io");

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);
        }

        private void CreateUser(string username, string password, string email) 
        {
            var connectionStringsSection = ConfigurationSource.Instance.Get<ConnectionStringsSection>("connectionStrings");
            string connectionString = connectionStringsSection.ConnectionStrings["EPiServerDB"].ConnectionString;

            using (UserStore<ApplicationUser> store = new UserStore<ApplicationUser>(
                new ApplicationDbContext<ApplicationUser>(connectionString)))
            {   
                if (!store.Users.Any(x => x.UserName == username))
                {
                    ApplicationUser user = Create(username, password, email, store);
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
