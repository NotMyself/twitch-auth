using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace app
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    public void ConfigureServices(IServiceCollection services)
    {
      services.Configure<CookiePolicyOptions>(options =>
      {
        options.CheckConsentNeeded = context => true;
        options.MinimumSameSitePolicy = SameSiteMode.None;
      });

      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultSignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = CookieAuthenticationDefaults.AuthenticationScheme;
      })
      .AddCookie()
      .AddOpenIdConnect("Auth0", options =>
      {
              // Set the authority to your Auth0 domain
              options.Authority = $"https://{Configuration["AUTH0_DOMAIN"]}";

              // Configure the Auth0 Client ID and Client Secret
              options.ClientId = Configuration["AUTH0_CLIENT_ID"];
        options.ClientSecret = Configuration["AUTH0_CLIENT_SECRET"];

              // Set response type to code
              options.ResponseType = "code";

              // Configure the scope
              options.Scope.Clear();
        options.Scope.Add("openid");

              // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
              // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
              options.CallbackPath = new PathString("/callback");

              // Configure the Claims Issuer to be Auth0
              options.ClaimsIssuer = "Auth0";

        options.Events = new OpenIdConnectEvents
        {
                // handle the logout redirection
                OnRedirectToIdentityProviderForSignOut = (context) =>
                 {
        var logoutUri = $"https://{Configuration["AUTH0_DOMAIN"]}/v2/logout?client_id={Configuration["AUTH0_CLIENT_ID"]}";

        var postLogoutUri = context.Properties.RedirectUri;
        if (!string.IsNullOrEmpty(postLogoutUri))
        {
          if (postLogoutUri.StartsWith("/"))
          {
                  // transform to absolute
                  var request = context.Request;
            postLogoutUri = request.Scheme + "://" + request.Host + request.PathBase + postLogoutUri;
          }
          logoutUri += $"&returnTo={ Uri.EscapeDataString(postLogoutUri)}";
        }

        context.Response.Redirect(logoutUri);
        context.HandleResponse();

        return Task.CompletedTask;
      }
        };
      });


      services.AddMvc()
        .SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
    }

    public void Configure(IApplicationBuilder app, IHostingEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
      }

      app.UseStaticFiles();
      app.UseCookiePolicy();
      app.UseAuthentication();

      app.UseMvc(routes =>
      {
        routes.MapRoute(
                  name: "default",
                  template: "{controller=Home}/{action=Index}/{id?}");
      });
    }
  }
}
