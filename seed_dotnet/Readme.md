# seed-dotnet — Seed for .NET Systelab projects

DRAFT

In this document described how is organized the .Net Core Web API seed

## Startup file

## Controllers

## Services

## Models



## ViewModels

The ViewModels allow you to shape multiple entities from one or more data models or sources into a single object, and use it as comunication with API.

There are not any combination of models in a view model in this seed, but we would like to contains the idea in the seed, to take into consideration this option for the new developments.

## DB

The seed have a integrated SQLLite database, where the information of the users and the patients will stored. This database is intended only for this seed project. In your future projects you can use the database you want.

These are the changes that you need to do to change the database:

- In the appsettings.json file set the new database connection string

```json
  "ConnectionStrings": {
    "seed_dotnetContextConnection": "Data Source=.\\db\\seed_dotnetdb.db;"
  },
 ```
 
 - In the seed_dotnetContext.cs change the database property of the option builder for the correct one
 
 ```c#
    optionsBuilder.UseSqlite(_config["ConnectionStrings:seed_dotnetContextConnection"]);
 ```
