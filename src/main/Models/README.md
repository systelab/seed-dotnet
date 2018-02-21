# Seed-dotnet Models

Models represents domain specific data and business logic in MVC architecture. It maintains the data of the application. Model objects retrieve and store model state in the persistance store like a database.

In this seed a part of the models which represent the database structure, also we add the models needed to add the authentication token in the header of the REST End points in Swagger page and the context definition.

## Models

### UserManage.cs

This model is a extend from the class IdentityUser, we extend the class to add more fields in the user profile, like the Name and the LastName.

### Patient.cs

This model represent the structure of the data which are going to save about the patients.

## Authentication Token in the header

### AddRequieredHeaderParameter.cs

This internal class extend from the Interface IOperationFilter provided by Swashbuckle (used to add Swagger in the solution).

The method Apply is overridden to add the parameter Authorization in the header of the REST End Points in the Swagger page.

### Parameter.cs

Is the structure of a parameter to add in the Swagger REST End points headers.

###JsonWebToken
This model represent the structure of the JWT token response.

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



