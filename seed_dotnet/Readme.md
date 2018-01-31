# seed-dotnet — Seed for .NET Systelab projects

In this document described how is organized the .Net Core Web API seed

## Startup file

Is the first file which is run when the application was launched. The main work of this file is about registering services and injection of modules in HTTP pipeline

- This file include a ConfigureServices method to configure the app's services.
- Must include a Configure method to create the app's request processing pipeline.

In our case the startup file as interesting parts contains:

### Configure the CORS
```c#
//Configure Services Method
//Allow use the API from other origins 
services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
{
    builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials();
}
));

//Configure method
  app.UseCors("MyPolicy");

```

To allow CORS in the controllers is added in the top of the controller this:

```c#
[EnableCors(origins: "*", headers: "*", methods: "*", SupportsCredentials = true)]

```

#### Configure the DB Context
```c#
services.AddDbContext<seed_dotnetContext>();
```
### Set the User Identity configuration
```c#
services.AddIdentity<UserManage, IdentityRole>(config =>
            {
                config.Password.RequireNonAlphanumeric = true;
                config.Password.RequiredLength = 8;
                config.Password.RequireDigit = true;
                config.User.RequireUniqueEmail = true;
            })
            .AddEntityFrameworkStores<seed_dotnetContext>();
```

### Configure the authentication system

```c#
services.AddAuthentication()
      .AddCookie()
      .AddJwtBearer(cfg =>
      {
          cfg.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
          {
              ValidateIssuer = false,
              ValidAudience = _config["Tokens:Audience"],
              IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Tokens:Key"]))
          };
      });
```        
### Configuration of swagger
```c#
//ConfigureServices method
 services.AddSwaggerGen(c =>
  {
      c.OperationFilter<AddRequiredHeaderParameter>();

      c.SwaggerDoc("v1", new Info
      {
          Version = "v1",
          Title = "Seed DotNet",
          Description = "This is a seed project for a .Net WebApi",
          TermsOfService = "None",

      });

      // Set the comments path for the Swagger JSON and UI.
      var basePath = PlatformServices.Default.Application.ApplicationBasePath;
      var xmlPath = Path.Combine(basePath, "seed_dotnet.xml");
      c.IncludeXmlComments(xmlPath);
  });
            
//Configure method
// Enable middleware to serve generated Swagger as a JSON endpoint.
    app.UseSwagger();

    // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.), specifying the Swagger JSON endpoint.
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Seed .Net");
    });
            
```  

### The model mapper
```c#
//Map the view model objet with the internal model
  Mapper.Initialize(config =>
  {
      config.CreateMap<PatientViewModel, Patient>().ReverseMap();
  });
``` 

## Controllers

The Controller responsibility is to handle the HTTP requests and responses.

## Services
A controller must delegate the domain logic to an external class; the service will play that role, also in this section we can include the repository pattern.

The role of the Repository is to access data, wherever is the data and in whatever format it could be. For the external world (the domain), the repository must deal in Domain Entities, making the rest of the system decoupled from its data source.

## Models

Model represents domain specific data and business logic in MVC architecture. It maintains the data of the application. Model objects retrieve and store model state in the persistance store like a database.

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
