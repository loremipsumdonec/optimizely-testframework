using EPiServer;
using EPiServer.Authorization;
using EPiServer.Core;
using EPiServer.DataAccess;
using EPiServer.Security;
using EPiServer.ServiceLocation;
using EPiServer.Shell.Security;
using Lorem.Models.Pages;
using Lorem.Test.Services;
using System;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace Lorem.Test
{
    [Collection("Default")]
    public class EpiserverEngineTestWithCollection
    {
        private EpiserverEngineSecondIteration<Startup> _engine;

        public EpiserverEngineTestWithCollection(EpiserverEngineSecondIteration<Startup> engine) 
        {
            _engine = engine;
            _engine.Start();
        }

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async void StartWithEpiserverEngineSecondIteration_CreateStartPage_HasPage(int _)
        {
            await CreateUser("Administrator", "Administrator123#", "loremipsumdonec@supersecretpassword.io", Roles.Administrators);

            IContentRepository repository = ServiceLocator.Current.GetInstance<IContentRepository>();

            var startPage = repository.GetDefault<StartPage>(ContentReference.RootPage);

            startPage.Name = "Start";
            startPage.Heading = "Welcome to Lorem";
            startPage.StartPublish = DateTime.Now;

            repository.Save(startPage, SaveAction.Publish, AccessLevel.NoAccess);

            var pages = repository.GetChildren<StartPage>(ContentReference.RootPage);
            Assert.Single(pages);
        }

        private async Task CreateUser(string username, string password, string email, params string[] roles) 
        {
            var userProvider = ServiceLocator.Current.GetInstance<UIUserProvider>();
            var roleProvider = ServiceLocator.Current.GetInstance<UIRoleProvider>();

            if (await userProvider.GetUserAsync(username) == null)
            {
                var result = await userProvider.CreateUserAsync(username, password, email, null, null, true);

                if (result.Status == UIUserCreateStatus.Success)
                {
                    foreach (string role in roles)
                    {
                        if (!await roleProvider.RoleExistsAsync(role))
                        {
                            await roleProvider.CreateRoleAsync(role);
                        }
                    }

                    await roleProvider.AddUserToRolesAsync(result.User.Username, roles);
                }
                else
                {
                    StringBuilder builder = new StringBuilder();

                    foreach (var error in result.Errors)
                    {
                        builder.AppendLine("* " + error);
                    }

                    throw new ArgumentException($"Failed creating user!\r\n{builder}");
                }
            }
        }
    }
}
