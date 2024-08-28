


# DertInfo - API

This is the API and business logic code for the DertInfo application.

The producton version of this solution is visible [[https://dertinfo-live-api-wa.azurewebsites.net/swagger/index.html](https://dertinfo-live-api-wa.azurewebsites.net/swagger/index.html)]

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

This project is a C# .NET API currently running .NET 8.0 

## Topology

![Application Containers](/docs/images/architecture-dertinfo-api-containerlevel.png)

## Installation

In order to get this project running locally you are going to need.

- Visual Studio Community
- Microsoft SQL Server Express


## Running The Project

You can run the API locally in 3 ways. 
- 1) (Visual Studio) Running as a isolated web app
- 2) (Visual Studio) Running as a container in docker desktop 
- 3) (Visual Studio) Running the docker componse project - This will spin up the entire estate using images on docker hub for the other parts. 
- 4) (Docker) Run the image using docker hub image
- 5) (Docker) Build and Run using the Dockerfile
- 6) (Docker Compose) Running the docker componse project (/infra/docker)  THis will spin up the API and all resoruces it depends on. 

```
4) > docker run dertinfo/dertinfo-api:latest 
5) src> docker build -t dertinfo/dertinfo-api -f dertinfo-api/Dockerfile .
6) infra/docker> docker-compose up 
```

When using docker compose if any container fails to start just restart the container. In some cases we can get some timing issues that we should resolve using health checks. 

**User Secrets**
```
{
  "StorageAccount:Images:Key": "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw==",
  "SqlConnection:ServerName": "[MY_LOCAL_IPv4_IP_ADDRESS],1433",
  "SqlConnection:ServerAdminPassword": "[MY_SQL_USER_PASSWORD]",
  "SqlConnection:ServerAdminName": "[MY_SQL_USER_USERNAME]",
  "SqlConnection:DatabaseName": "[MY_SQL_DATABASE_NAME]",
  "SendGrid:ApiKey": "[MY_SENDGRID_ACCESS_KEY_FOR_SENDING_EMAILS]"",
  "PwaClient:Auth0:ClientId": "[MY_AUTH0_APP_CLIENT_ID]",
  "WebClient:Auth0:ClientId": "[MY_AUTH0_WEB_CLIENT_ID]",
  "Kestrel:Certificates:Development:Password": "[KESTRAL_UUID_PASSWORD]",
  "Auth0:ManagementClientSecret": "[MY_AUTH0_MANAGEMENT_CLIENT_SECRET]",
  "Auth0:Domain": "dertinfodev.eu.auth0.com",
  "ApiInfo:ContactName": "David Hall",
  "ApiInfo:ContactEmail": "dertinfo@gmail.com"
}
```
You will need to put this file in your visual studio user secrets in order that it doesn't get checked and exposes secrets. 

To note with this configutation: 
- Auth0:ManagementClientId & Secret - These values are to a special endpoint at Auth0 that allows the manipulation of users in the tenant. We use this endpoint to apply access to user accounts that give them permissions to view groups, events, venues etc. 
- The StorageAccount references are those for the Azure Storage Emulator and the information is not secret [Azure Storage Explorer Docs](https://learn.microsoft.com/en-us/azure/storage/common/storage-use-emulator)
- The api infomation is what is displayed when veiwing the [swagger](https://swagger.io/tools/swaggerhub) definitions for the API. 
- Sendgrid enabled as FALSE is a setting that stops emails being sent in a given environment. 

> **note:** Please see documentaion/wiki for the values to add to the Json for the Auth0. Also provided are a number of logins that will allow access to preconfigured data to support contibution. 

> **sensitive** Setting up the Auth0 accounts from scratch is an extensive process and requires some detailed configuration. To request the managementclient secret for the development and test envionments please email [dertinfo@gmail.com](mailto:dertinfo@gmail.com) once recieved this will allow you use the development and test Auth0 tenants for the project. 

### To run the api.

#### Settings up the database
You will need to create the database and roll forward the migrations on your first use of the API. 

(Coming soon) It'll be useful to be able to seed the database with a given seed sets appropraite for testing or working with different environments. 

#### From Visual Studio


## Usage

When the application is successfully running it will provide the endpoints used for both the web and app clients. 

You can inspect the endpoints and models via the swagger definitions at the path /swagger/index.html (e.g. http://localhost:12345/swagger/indes.html)

## Features

(API Features documentation to be completed later)

## Contributing

Please refer to [CONTRIBUTING.md](/CONTRIBUTING.md) and [CODE_OF_CONDUCT.md](/CODE_OF_CONDUCT.md) for information on how others can contribute to the project.

## License

This project is licenced under the GNU_GPLv3 licence. Please refer to the [LICENCE.md](/LICENCE.md) file for more information. 
