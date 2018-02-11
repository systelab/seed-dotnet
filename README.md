# seed-dotnet — Seed for .NET Systelab projects

[![Codacy Badge](https://api.codacy.com/project/badge/Grade/e4aa345edeef415abe5ccab9664079b7)](https://app.codacy.com/app/alfonsoserra/seed-dotnet?utm_source=github.com&utm_medium=referral&utm_content=systelab/seed-dotnet&utm_campaign=badger)

This project is an application skeleton for a typical .NET Core WEB API application. You can use it
to quickly bootstrap your projects and dev environment.

The seed contains a Patient Management sample Web API.

The app doesn't do much, just shows how to use different .NET Core patterns and other suggested tools together:

* .NET Core 2.0.0
* Entity Framework Core 6.0
* JWT
* CORS
* Swagger
* AutoMapper
* Local database
* Logging
* xUnit with Moq


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

### Run

To run the project, press the run button provided by Visual Studio. The browser will be opened with the included swagger page. The start point can be changed in the 'launchSettings.json'.

### How it works

After login (with username **quentinada** and password **P@ssw0rd!**), copy the Token returned in the Authorization field before running any other REST end point.

## Improvements

The project is not finished, E2E test is not still implemented. We are working hard to have this implemented as soon as possible. Also the idea is to include Allure to show the test results in a proper maner.

[git]: https://git-scm.com/
[dotnet]:https://www.microsoft.com/net/download/windows

