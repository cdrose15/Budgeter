using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.Google;
using Owin;
using Budgeter.Models;
using Owin.Security.Providers.LinkedIn;
using Owin.Security.Providers.GitHub;

namespace Budgeter
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(ApplicationDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });            
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            app.UseMicrosoftAccountAuthentication(
                clientId: "000000004417C021",
                clientSecret: "mpzPpPhYhWDNmuRGowFJcXoMB6tTbSWF");

            app.UseTwitterAuthentication(
               consumerKey: "PWmtzEvBdTMfJ2GJWBDA1WEny",
               consumerSecret: "p7z1mcC5wsT1mdsiRVkio93M3hRdeNjQPP8ifpfmfTh8Lko3Uw");

            app.UseFacebookAuthentication(
               appId: "1679103069041937",
               appSecret: "5fe2a7ca3317a1fe43ee538fc972c481");

            app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            {
                ClientId = "795097649471-u3lc8eva98ho7lpk6a95kvhs463ddltr.apps.googleusercontent.com",
                ClientSecret = "aszuOjxGqphSJ8DE13d6nu_u"
            });

            app.UseLinkedInAuthentication("77cqvvlk8h3zrt", "OeXSfuGYk15VpDer");

            app.UseGitHubAuthentication(
                clientId: "a7764ab9ebbf06896f50",
                clientSecret: "db5bf16e5a1481c1d2eb4e3c4828de7d72d931dc");
        }
    }
}