[![Codacy Badge](https://api.codacy.com/project/badge/Grade/d6124578c1984b24bde396b8ada17d0e)](https://www.codacy.com/app/alfonsoserra/seed-dotnet?utm_source=github.com&amp;utm_medium=referral&amp;utm_content=systelab/seed-dotnet&amp;utm_campaign=Badge_Grade)
[![Build Status](https://travis-ci.org/systelab/seed-dotnet.svg?branch=master)](https://travis-ci.org/systelab/seed-dotnet)

# seed-dotnet — Seed for .NET Systelab projects

This project is an application skeleton for a typical .NET Core WEB API application. You can use it
to quickly bootstrap your projects and dev environment.

The seed contains a Patient Management sample Web API.

The app doesn't do much, just shows how to use different .NET Core patterns and other suggested tools together:

* .NET Core 2.2.0
* Entity Framework Core 6.0
* JWT
* CORS
* Swagger
* AutoMapper
* Local database
* Logging
* xUnit with Moq
* Allure


## Getting Started

To get you started you can simply clone the `seed-dotnet` repository.

### Prerequisites

You need [git][git] to clone the seed-dotnet repository.
In order to build the application you will need Visual Studio 2017 and [.Net Core][dotnet].

### Clone `seed-dotnet`

Clone the `seed-dotnet` repository using git:

```bash
git clone https://github.com/systelab/seed-dotnet.git
cd seed-dotnet
```

If you just want to start a new project without the seed-dotnet commit history then you can do:

```bash
git clone --depth=1 https://github.com/systelab/seed-dotnet.git <your-project-name>
```

The depth=1 tells git to only pull down one commit worth of historical data.


### Open the Visual Studio solution

Once you have the repository cloned, open the visual studio solution 'seed_dotnet.sln'

The solution contains the Web API and the Unit Test project (unfinished).

### How to install Allure

First you should install **"scoop"**, follow the steps described in this link: [scoop]
After the installation have been finished, execute the follow commands:
```bash
scoop install allure
```
If you already have installed Allure and you want to update the application execute the follow command:
```bash
scoop update allure
```

### Run

You have two options:

#### Use the scripts provided

You have two scripts, one is to run the project [app] (use this option to integrate the FrontEnd that you want) and the other is to run all the tests and view the results in the allure application [test].

To acceess to swagger: http://127.0.0.1:13080/swagger/

#### Use Visual Studio
To run the project, press the run button provided by Visual Studio. The browser will be opened with the included swagger page. The start point can be changed in the 'launchSettings.json'.


### How it works

After login (with username **Systelab** and password **Systelab**), copy the Token returned in the Authorization field before running any other REST end point.

## HTTPS, Angular and Chrome with local trust certificate

Chrome rejects automatically requests made from Angular to endpoints with untrusted certificates.

To accept local signed certificates on development, paste this url on navigation bar chrome://flags/#allow-insecure-localhost and enable the setting **"Allow invalid certificates for resources loaded from localhost"**

## Docker

### Build docker image

There is an Automated Build Task in Docker Cloud in order to build the Docker Image. 
This task, triggers a new build with every git push to your source code repository to create a 'latest' image.
There is another build rule to trigger a new tag and create a 'version-x.y.z' image

You can always manually create the image with the following command:

```bash
docker build -t systelab/seed-dotnet . 
```

The image created, will contain the deployment of the aspnetcore application

### Run the container

```bash
docker run -p 13080:13080 systelab/seed-dotnet
```

The app will be available at http://localhost:13080

# Documentation
See [Documentation](doc/README.md) section for further details about other technical specifications.

[git]: https://git-scm.com/
[dotnet]:https://www.microsoft.com/net/download/windows
[scoop]:http://scoop.sh/
[test]:https://github.com/systelab/seed-dotnet/blob/master/src/test.bat
[app]:https://github.com/systelab/seed-dotnet/blob/master/src/app.bat