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
1. Run the command `dotnet user-secrets set AUTH0_API_CLIENT_ID {auth0-management-api-client-id}`.
1. Run the command `dotnet user-secrets set AUTH0_API_CLIENT_SECRET {auth0-management-api-client-secret}`.
1. Return to the root of the repository `popd`.
1. Run the command `dotnet run --project src/app.` to start the application from the command line.
1. Optionally, open in Visual Studio Code and run with the debugger.
