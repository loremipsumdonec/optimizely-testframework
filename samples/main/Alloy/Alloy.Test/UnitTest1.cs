using AlloyTemplates.Models.Pages;
using EPiServer;
using EPiServer.Authorization;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using EPiServer.Templates.Alloy.Mvc;
using Lorem.Testing.EPiServer.CMS;
using System;
using Xunit;

namespace Alloy.Test
{
    public class UnitTest1
    {
        private readonly EpiserverEngine<Startup> _engine;

        public UnitTest1()
        {
            _engine = new();
        }

        [Theory]
        [InlineData(1)]
        public async void Test1(int _)
        {
            _engine.CreateClient();

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            CreateUser("Administrator", "Administrator2745#", "loremipsumdonec@supersecretpassword.io");

            startPage.Name = "Start";
            startPage.MetaDescription = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);

            Assert.NotNull(repository);
        }

        private async void CreateUser(string username, string password, string email)
        {
            UIUserProvider userProvider = ServiceLocator.Current.GetInstance<UIUserProvider>();
            UIRoleProvider roleProvider = ServiceLocator.Current.GetInstance<UIRoleProvider>();

            if(await userProvider.GetUserAsync(username) == null) 
            {
                var result = await userProvider.CreateUserAsync(username, password, email, null, null, true);

                if (result.Status == UIUserCreateStatus.Success)
                {
                    if(!await roleProvider.RoleExistsAsync(Roles.WebAdmins)) 
                    {
                        await roleProvider.CreateRoleAsync(Roles.WebAdmins);
                    }

                    await roleProvider.AddUserToRolesAsync(result.User.Username, new string[] { Roles.WebAdmins });
                }
            }
        }
    }
}
