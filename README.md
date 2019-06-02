# Twitch Authentication

This is a sample application that demonstrates how to build an ASP.NET MVC application that uses Twitch for authentication while also providing Twitch API access. The application assumes you are using Auth0 for Authentication and Authorization services. Auth0 offers a [free tier](https://auth0.com/pricing) that is perfect for hobby projects.

## Getting Started

### Running Locally

As long as you have the development dependencies installed, the application can be run on your bare metal machine.

1. Clone the repository: `git clone https://github.com/NotMyself/twitch-auth.git`.
1. Change directory into the cloned repository `cd twitch-auth`.
1. Temporarily change directory `pushd src/app/`.
1. Run the command `dotnet user-secrets set AUTH0_DOMAIN {auth0-tenant-domain}`.
1. Run the command `dotnet user-secrets set AUTH0_CLIENT_ID {auth0-app-client-id}`.
1. Run the command `dotnet user-secrets set AUTH0_CLIENT_SECRET {auth0-app-client-secret}`.
1. Return to the root of the repository `popd`.
1. Run the command `dotnet run --project src/app.` to start the application from the command line.
1. Optionally, open in Visual Studio Code and run with the debugger.


## Provisioning and Configuring Services

### Create an Auth0 Tenant

First you need an Auth0 account.

1. Create a free account with [Auth0](https://auth0.com/signup).
  - Note your tenant name, you will need it in the next step.


### Setting up Twitch

First you will need to register an application with Twitch.

1. Register a new application on the [Twitch Developer Console](https://dev.twitch.tv/console).
1. Use `https://YOUR_AUTH_TENANT_NAME.auth0.com/login/callback` for the **OAuth Redirect URL**.
1. Use Application Integration as the **Category**.
1. Note the **Client ID and Client Secert**, you will need it next.

### Obtaining Auth0 Client Credentials

Then, you need to create a new Application to use for OpenID Connect based authentication.

1. From the Auth0 Dashboard, click the **Create Application** button.
1. Name it **Twitch Authentication**, and select **Regular Web Applications** as the application type.
1. Click the **Create** button.
1. Select the **Settings Tab**.
1. Store your **Domain**, **Client ID**, and **Client Secret** using the `dotnet user-secrets` commands above.
  - **Note:** These are the values for `AUTH0_DOMAIN`, `AUTH0_CLIENT_ID`, and `AUTH0_CLIENT_SECRET`.

### Setting up Twitch Custom Social Connection

Then, Create a **Custom Social Connection** for Twitch in Auth0.

1. From the Auth0 Dashboard, select the **Extensions** menu item in the left hand nav.
1. Install the **Custom Social Connection** extension.
1. Enable the Twitch connection.
1. Supply your Twitch application credentials.
1. Ensure the **Twitch Authentication** application is allowed to use this connection on the **Apps** tab.
1. Ensure authentication works, by clicking the **try** button.

### Adding Auth0 Rules

Next, the access tokens issued to Auth0 by external Identity Providers are stored in the user profile. But they are not sent out to clients by default. A rule is needed to enrich the issued id token with this data.

**Note:** This is a short cut method of building this functionality. It is somewhat secure because we are using a server side rendered framework. Care should be taken to never leak these keys to the users browser or devices directly.

1. From the Auth0 Dashboard, select the **Rules** menu item in the left hand nav.
1. Click the **Create Rule** button
1. Select the **Empty Rule** template.
1. Name the rule **Add IDP Access Tokens as Claims**.
1. Copy the contents of the `src/rules/addIdpsRule.js` into the editor.
1. Click the **Save** button.

### Enable the Rule for the Client

Finally, the rule we created in the last section has a safeguard clause in line 6. It check client metadata for a opt in flag to enable the rule. This is primarily used to ensure clients are not accidentally including IDP tokens in their id tokens. Clients must opt in to the rule.

1. From the Auth0 Dashboard, select the **Applications** menu item in the left hand nav.
1. Select the **Twitch Authentication** application we created earlier.
1. On the **Settings** tab, scroll down and select **Show Advanced Settings**.
1. On the **Application Metadata** tab add a `includeIdps` key with the value set to `true`.
1. Click the **Save Changes** button.
