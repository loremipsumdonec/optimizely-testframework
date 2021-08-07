using EPiServer.Authorization;
using EPiServer.Templates.Alloy.Mvc;
using Lorem.Testing.EPiServer.CMS;
using System;
using System.Globalization;

namespace Alloy.Test.Services
{
    public class DefaultEpiserverFixture
        : EpiserverFixtureWithFactory<Startup>
    {
        public DefaultEpiserverFixture(EpiserverEngine<Startup> engine)
        {
            Engine = engine;
            Engine.Start();

            Cultures.Add(CultureInfo.GetCultureInfo("en"));

            Register("episerver.site.name", "lorem");
            Register("episerver.site.url", new Uri("http://localhost:57728/"));
            Register("episerver.site.language", CultureInfo.GetCultureInfo("en"));

            CreateUser("Administrator", "Administrator1234!", "loremipsumdonec@supersecretpassword.io", Roles.Administrators);
        }
    }
}
