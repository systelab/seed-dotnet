# seed-dotnet — Seed for .NET Systelab projects

This project is an application skeleton for a typical .NET Core WEB API application. You can use it
to quickly bootstrap your projects and dev environment.

The seed contains a Patient Management sample Web API.

The app is based in a common patterns of developments in .NET Core, the app use the follow:

* .NET Core 2.0.0
* Entity Framework Core 6.0
* JWT
* CORS
* Swagger
* AutoMapper
* Local database
* Logging

Refering to unit tests, the project is not finished, but the idea is to use JustMock We are working to have this ready as soon as possible. Also the idea is to include in the solution the Allure solution to show the results of the unit test results.

## Getting Started

To get you started you can simply clone the `seed-dotnet` repository:

### Prerequisites

- Have installed a Visual Studio 2017 
- Have installed .Net Core (you can download from [dotnet][dotnet])
- Have installed [git][git]  to clone the `seed-dotnet` repository

### Clone `seed-dotnet`

Clone the `seed-dotnet` repository using git:

```bash
git clone https://github.com/systelab/seed-dotnet.git
cd seed-dotnet
```

The `depth=1` tells git to only pull down one commit worth of historical data.

### Open the Visual Studio solution

Once you have the repository cloned, open the cloned visual studio solution 'seed_dotnet.sln'

The solution contains the Web API and the Unit Test project (unfinished).

### Run

To see the result, run the project with the run button provided by Visual Studio. the browser will be opened with the included swagger solution (The start point could be change it in the 'launchSettings.json').

### How it works without UI

- Do a login (username= admin and password= P@ss0rd!)
- Copy the Token returned
- Use the other End points to retrive, add, update or delete patients, but REMEMBER in the Authorization field add "Bearer {Token that you get before}"

As always if you know how to do better or if you want to help us to make a good seed let us know!!!

[git]: https://git-scm.com/
[dotnet]:https://www.microsoft.com/net/download/windows
