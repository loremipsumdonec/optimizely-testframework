using EPiServer.Shell.Security;
using System.Threading.Tasks;
using EPiServer.ServiceLocation;
using System.Collections.Generic;
using System;
using System.Text;

namespace Lorem.Testing.Optimizely.CMS.Commands
{
    internal class CreateUser
    {
        private readonly UIUserProvider _userProvider;
        private readonly UIRoleProvider _roleProvider;

        public CreateUser(string username, string password, string email, params string[] roles)
            : this(
                  username,
                  password,
                  email,
                  roles,
                  ServiceLocator.Current.GetInstance<UIUserProvider>(),
                  ServiceLocator.Current.GetInstance< UIRoleProvider>()
                )
        {
            Username = username;
            Password = password;
            Email = email;
        }

        public CreateUser(
            string username,
            string password,
            string email, 
            string[] roles,
            UIUserProvider userProvider,
            UIRoleProvider roleProvider)
        {
            Username = username;
            Password = password;
            Email = email;
            Roles = new List<string>(roles);
            _userProvider = userProvider;
            _roleProvider = roleProvider;
        }

        public string Username { get; set; }

        public string Password { get; set; }

        public string Email { get; set; }
    
        public List<string> Roles { get; set; }

        public void Execute()
        {
            ExecuteAsync()
                .GetAwaiter()
                .GetResult();
        }

        private async Task ExecuteAsync() 
        {
            if (await _userProvider.GetUserAsync(Username) == null)
            {
                var result = await _userProvider.CreateUserAsync(Username, Password, Email, null, null, true);

                if (result.Status == UIUserCreateStatus.Success)
                {
                    foreach(string role in Roles)
                    {
                        if (!await _roleProvider.RoleExistsAsync(role))
                        {
                            await _roleProvider.CreateRoleAsync(role);
                        }
                    }

                    await _roleProvider.AddUserToRolesAsync(result.User.Username, Roles);
                } 
                else 
                {
                    StringBuilder builder = new StringBuilder();

                    foreach(var error in result.Errors)
                    {
                        builder.AppendLine("* " + error);
                    }

                    throw new ArgumentException($"Failed creating user!\r\n{builder}");
                }
            }
        }
    }
}
