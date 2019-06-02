# Twitch Auth

A sample application that shows how to do Twitch Authentication with Auth0.

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


## Setting Up Auth0 and Obtaining Keys

First you need an Auth0 account.

1. Create a free account with [Auth0](https://auth0.com/signup).
  - Note your tenant name, you will need it in the next step.

Next, you will need to register your application with Twitch.

1. Register a new application on the [Twitch Developer Console](https://dev.twitch.tv/console).
1. Use `https://YOUR_AUTH_TENANT_NAME.auth0.com/login/callback` for the **OAuth Redirect URL**.
1. Use Application Integration as the **Category**.
1. Note the **Client ID and Client Secert**, you will need it next.

Then, you need to create a new Application to use for OpenID Connect based authentication.

1. From the Auth0 Dashboard, click the **Create Application** button.
1. Name it **Twitch Authentication**, and select **Regular Web Applications** as the application type.
1. Click the **Create** button.
1. Select the **Settings Tab**.
1. Store your **Domain**, **Client ID**, and **Client Secret** using the `dotnet user-secrets` commands above.
  - **Note:** These are the values for `AUTH0_DOMAIN`, `AUTH0_CLIENT_ID`, and `AUTH0_CLIENT_SECRET`.

Then, Create a **Custom Social Connection** for Twitch in Auth0.

1. From the Auth0 Dashboard, select the **Extensions** menu item in the left hand nav.
1. Install the **Custom Social Connection** extension.
1. Enable the Twitch connection.
1. Supply your Twitch application credentials.
1. Ensure the **Twitch Authentication** application is allowed to use this connection on the **Apps** tab.
1. Ensure authentication works, by clicking the **try** button.
