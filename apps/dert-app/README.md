# DertInfo - App

This is the mobile application for DertInfo that is used for a number of purposes but primariliy for collecting scores at venue and taking photographs of the score sheets for varification. 

The producton version of this solution is visible at [https://app.dertinfo.co.uk](https://app.dertinfo.co.uk)

> **Note:** If you are unfamilar with the collection of services that are part of DertInfo please refer to the repository dertinfo/dertinfo.

## Table of Contents

- [Technology](#technology)
- [Topology](#topology)
- [Installation](#installation)
- [Usage](#usage)
- [Features](#features)
- [Contributing](#contributing)
- [License](#license)

## Technology

This project is a ["Azure Static Web App"](https://azure.microsoft.com/en-gb/products/app-service/static). 

This static web app consists of an ["Angular"](https://angular.dev/) client and a typescript ["function app"](https://azure.microsoft.com/en-gb/products/functions) combined into a single deployable unit where the running application is configurable from settings stored in ["Azure App Configuration"](https://azure.microsoft.com/en-us/products/app-configuration) and ["Azure Key Vault"](https://azure.microsoft.com/en-us/products/key-vault). 

## Topology

![Application Containers](/docs/images/architecture-dertinfo-app-containerlevel.png)

## Installation

In order to get this project running you are going to need.

- The latest version of [Visual Studio Code](https://code.visualstudio.com/) installed.
- The [Azure Static Web Apps CLI](https://azure.github.io/static-web-apps-cli/) installed 
- The [NodeJs](https://nodejs.org/en/download/package-manager) version to support Angular
- The [Angular CLI](https://v17.angular.io/guide/setup-local) installed 

> **Note:** If you need multiple versions of node installed to support multiple projects consider using [nvm](https://github.com/nvm-sh/nvm).


## Running The Project

### Codespaces

This app has been fully setup for codespaces and is by far the easiest way to get started. 

**Launch the codespace:**

![image](https://github.com/user-attachments/assets/f4ac2952-9762-4cf5-9eac-c7b7585ec292)

Let the codespace setup by applying the VS code extensions and allow the startup script s to run. (codespace defined in .devcontainer). This will setup the environment for running the app and it's dependencies.

**Launch the dependencies**

In the VSCode teminal navigate to infra/docker and run docker-compose up

```
cd infra/docker
docker-compose up
```

This will pull all the dependencies from docker hub and setup the whle envionment please give it time on first setting up your code space this can take upto 5 mins. (See /infra/docker/setup-codespace.sh)

If any dependency doesn't start simply restart it as it'll be a timing issue on launch. 

Make sure that your APP and API ports are publically visible and if thay are not make sure they are. (They need to be public for the Auth mechainsms to work in codespaces)

![image](https://github.com/user-attachments/assets/350499ed-4389-4f0e-bcc5-f7bbe5eb0448)

For More information on how we use "docker" and "docker-compose" in DertInfo please see the wiki [here](https://github.com/dertinfo/dertinfo/wiki/How-we-use-docker-&-docker%E2%80%90compose-in-DertInfo)

**Check your environment**

The app will now be running on port :44300 within the codespace you can access it by looking at the forwarded ports in VS Code and proceeding. 

You may get a warning about accessing the codespace simply continue.

You should gain the homepage of the App. (Please note you may need to adjust the view port to mobile)

Please note that functionality will be limited as there will be no data. Please see #

### Local Development

In order for this client to work in your local environment you will need to have the following services available:
- The DertInfo API running for access to the application data. 
- The client component of static web app for the intereface
- The function app component of the static web app to get the configuration
- The static web app itself to bind the client and the function app. 

### For the function app

You will need to first of all add a local settings file to your project. It is ignored by the git ignore hence you will not find it in the project. This file will contain settings that will allow the app to communicate to the configuration & the authentication services. 

The following file needs to be added to the location of the function app project. It will specify the envionment variables for the locally running function app. 

Create the file local.settings.json at the location /src/functions/ with the default settings below. 

**local.settings.json**
```
{
    "IsEncrypted": false,
    "Values": {
        "AzureWebJobsStorage": "",
        "FUNCTIONS_WORKER_RUNTIME": "node",
        "AZURE_APP_CONFIG": "",
        "ENVIRONMENT": "Development",
        "devonly_apiUrl": "MY_API_URL_HERE",
        "devonly_auth0ClientId": "AUTH0_CLIENTID_HERE",
        "devonly_auth0TenantDomain": "AUTH0_TENANT_DOMAIN_HERE",
        "devonly_auth0Audience": "AUTH0_AUDIENCE_HERE",
        "devonly_auth0CallbackUrl": "http://localhost:4280",
        "devonly_appInsightsTelemetryKey": ""
    }
}
```
To note with this configutation: 
- The setting AZURE_APP_CONFIG is empty. This allows the application when running locally to use the "devonly_" envionment variables as opposed to collecting them from Azure App Configuration. 
- The "ENVIRONMENT" is tagged as "Development" for running locally. 
- The setting "devonly_apiUrl" is the URL of a locally running API. This URL must be an API configured in the same envionment as the client app. (e.g. Development) else authentication between the client and API will fail. 
- The AUTH0_ settings are those for connecting to the Auth0 tenant and allows logging in using "Authorization Code Flow with Proof Key for Code Exchange (PKCE)"

> **note:** Please see documentaion/wiki for the values to add to the Json for the Auth0. Also provided are a number of logins that will allow access to preconfigured data to support contibution.

### To run the client (Angular)
In a command window at "/src/client"
```
ng serve
```
> The angular client should start on the standard angular port of http://localhost:4200

### To run the function app
In a command window at "/src/functions"
```
func start --port 7082
```
> The function app should start on the (non standard) port 7082

### To link them together as the static web app
In a command window at "/src"
```
swa start http://localhost:4200 --port 4290 --api-port 7082
```
> The running static web app should be available at http://localhost:4290

### To run the api.
When it comes to getting the data for the app there are a few options. 
- You can run the API locally by running the project at [dertinfo-api](https://github.com/dertinfo/dertinfo-api) locally in Visual Studio. 
- (Coming soon) You can connect to the hosted development services at https://dertinfo-dev-api-wa.azurewebsites.net.
- (Coming soon) You can deploy your own version and dependencies into your own Azure Subscription using the supplied Infrastucture As Code
- (Coming soon) You can run the API and dependencies as local containers. 


## Usage

When users launch the application they will see the homepage of the application which looks like this: 

![Screenshot of the homepage](/docs/images/screenshot-homepage-nouser.png)

Users can login to the application using th icon in the top right which will rediect he user a third party authentication system of [Auth0](https://auth0.com/). The user can login or signup.

Once there is a known user application will then confiure itself for appropraitely for the user and allow further actions. Please refer to the documentaiton on GitHub wiki for more usage information. 

## Features

These are the key functions of this app: 

#### For Venue Admins
- View the dances that are expected, completed or checked at thier venue(s)
- Enter dance scores and take pictures of scoresheets
- Edit dance scores and scoresheets until dance is checked

#### For Teams Admins
- On results published view individual scores and scoresheetsfor each dance

#### For Team Members
- Login to access scores and scoresheets for thier team. 
- Signup to further enter team pin to view scores and scoresheets for thier team. 

## Contributing

Please refer to [CONTRIBUTING.md](/CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](/CODE_OF_CONDUCT.md) for information on how others can contribute to the project.

## License

This project is licenced under the GNU_GPLv3 licence. Please refer to the [LICENCE.md](/LICENCE.md) file for more information. 
