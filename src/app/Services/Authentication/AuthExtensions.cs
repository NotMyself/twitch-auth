using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace App.Services.Authentication
{
  public static class AuthExtensions
  {
    public static AuthenticationBuilder AddAuth0OpenIdConnect(this AuthenticationBuilder builder, IConfiguration config)
    {
      var domain = $"https://{config["AUTH0_DOMAIN"]}";
      var clientId = config["AUTH0_CLIENT_ID"];
      var clientSecret = config["AUTH0_CLIENT_SECRET"];

      return builder.AddOpenIdConnect("Auth0", options =>
      {
        // Set the authority to your Auth0 domain
        options.Authority = domain;

        // Configure the Auth0 Client ID and Client Secret
        options.ClientId = clientId;
        options.ClientSecret = clientSecret;

        // Set response type to code
        options.ResponseType = "code";

        // Configure the scope
        options.Scope.Clear();
        options.Scope.Add("openid");
        options.Scope.Add("profile");
        options.Scope.Add("email");
        options.Scope.Add("https://iamnotmyself.com/connections");

        options.TokenValidationParameters = new TokenValidationParameters
        {
          NameClaimType = "name"
        };

        options.GetClaimsFromUserInfoEndpoint = true;

        // Set the callback path, so Auth0 will call back to http://localhost:3000/callback
        // Also ensure that you have added the URL as an Allowed Callback URL in your Auth0 dashboard
        options.CallbackPath = new PathString("/callback");

        // Configure the Claims Issuer to be Auth0
        options.ClaimsIssuer = "Auth0";
        options.SaveTokens = true;

        options.Events = new OpenIdConnectEvents
        {
          // handle the logout redirection
          OnRedirectToIdentityProviderForSignOut = (context) =>
           {
             var logoutUri = $"https://{config["AUTH0_DOMAIN"]}/v2/logout?client_id={config["AUTH0_CLIENT_ID"]}";
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
    }
  }
}
