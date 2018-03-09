# Seed-dotnet Models

Models represents domain specific data and business logic in MVC architecture. It maintains the data of the application. Model objects retrieve and store model state in the persistance store like a database.

In this seed a part of the models which represent the database structure, also we add the models needed to add the authentication token in the header of the REST End points in Swagger page and the context definition.

## Models

### UserManage.cs

This model is a extend from the class IdentityUser, we extend the class to add more fields in the user profile, like the Name and the LastName.

### Patient.cs

This model represent the structure of the data which are going to save about the patients.

###JsonWebToken
This model represent the structure of the JWT token response.

## Swagger

### Consume Content types overrride

There are two class "SwaggerConsumesAttribute" and "SwaggerConsumesOperationFilter" used to add as decorator the content types to consume the endpoint.

## Context

### seed_dotnetContext.cs

In this class is defined the DBsets that are used and the database provider.
```c#
public DbSet<Patient> Patients { get; set; }
 ```
```c#
 optionsBuilder.UseSqlite(_config["ConnectionStrings:seed_dotnetContextConnection"]);
 ```
  
### seed_dotnetContextSeeData.cs

This class es used to fill the database when the application is launched.

This is usefull when you have data master needed in the database, and you want to ensure that the data is stored always in the database.



