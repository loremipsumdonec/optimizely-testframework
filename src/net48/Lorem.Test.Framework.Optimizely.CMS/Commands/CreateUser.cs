using EPiServer.Cms.UI.AspNetIdentity;
using EPiServer.Framework.Configuration;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;

namespace Lorem.Test.Framework.Optimizely.CMS.Commands
{
    internal class CreateUser
    {
        public CreateUser(string username, string password, string email, params string[] roles)
        {
            Username = username;
            Password = password;
            Email = email;
            Roles = new List<string>(roles);
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
                    AddRolesToUser(Roles, user, store, connectionString);

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
