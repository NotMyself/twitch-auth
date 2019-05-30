using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Auth0.AuthenticationApi;
using Auth0.AuthenticationApi.Models;
using Auth0.ManagementApi;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace App.Services.Authentication
{
  public class ApiTokenResolver
  {
    private readonly string domain;
    private readonly string clientId;
    private readonly string clientSecret;

    private string token = string.Empty;

    public ApiTokenResolver(string domain, string clientId, string clientSecret)
    {
      this.domain = domain ?? throw new ArgumentNullException(nameof(domain));
      this.clientId = clientId ?? throw new ArgumentNullException(nameof(clientId));
      this.clientSecret = clientSecret ?? throw new ArgumentNullException(nameof(clientSecret));
    }

    public async Task<string> ResolveAsync()
    {
      if (!string.IsNullOrWhiteSpace(token))
        return token;

      var authenticationApiClient = new AuthenticationApiClient(new Uri(domain));

      // Get the access token
      var res = await authenticationApiClient.GetTokenAsync(
        new ClientCredentialsTokenRequest
        {
          ClientId = clientId,
          ClientSecret = clientSecret,
          Audience = $"{domain}/api/v2/"
        });

      return token = res.AccessToken;
    }
  }
  public static class AuthExtensions
  {
    public static void AddManagementApiClient(this IServiceCollection services, IConfiguration config)
    {
      var domain = $"https://{config["AUTH0_DOMAIN"]}";
      var clientId = config["AUTH0_API_CLIENT_ID"];
      var clientSecret = config["AUTH0_API_CLIENT_SECRET"];

      services.AddSingleton<ApiTokenResolver>(c =>
      {
        return new ApiTokenResolver(domain, clientId, clientSecret);
      });

      services.AddSingleton<ManagementApiClient>(c =>
      {
        var resolver = c.GetService<ApiTokenResolver>();
        var token = resolver.ResolveAsync()
                            .GetAwaiter()
                            .GetResult();
        return new ManagementApiClient(token, $"domain/api/v2");
      });
    }
    public static AuthenticationBuilder AddAuth0OpenIdConnect(this AuthenticationBuilder builder, IConfiguration config)
    {
      var domain = $"https://{config["AUTH0_DOMAIN"]}";
      var clientId = config["AUTH0_CLIENT_ID"];
      var clientSecret = config["AUTH0_CLIENT_SECRET"];
      var logoutUri = $"https://{config["AUTH0_DOMAIN"]}/v2/logout?client_id={config["AUTH0_CLIENT_ID"]}";

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
