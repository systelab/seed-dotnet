# seed-dotnet — Seed for .NET Systelab projects

This document described how is organized the .Net Core Web API seed

## Startup file

Is the first file which is run when the application was launched. The main work of this file is about registering services and injection of modules in HTTP pipeline

- This file include a ConfigureServices method to configure the app's services.
- Must include a Configure method to create the app's request processing pipeline.

In our case the startup file as interesting parts contains:

### Configure the HTTPS usage and redirection

When using HTTPS the HTTP to HTTPS redirection must also be configured for those clients that try to access through HTTP

[Enforcing HTTPS in ASP.NET Core](https://docs.microsoft.com/es-es/aspnet/core/security/enforcing-ssl?view=aspnetcore-2.2&tabs=visual-studio)

At `Program.cs`

```c#
public static IWebHost BuildWebHost(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSetting("https_port", "13080")  // HTTPS port to use             
                .Build();
```

At `Startup.cs`

```c#
 public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            ILoggerFactory factory)
        {
		/// Other configurations

		app.UseHttpsRedirection();

		/// More configurations 

		}
```

At `launchSettings.json`

```javascript
// ...
"applicationUrl": "https://localhost:13080/"
// ...
```

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
[EnableCors("MyPolicy")]

```

#### Configure the DB Context
```c#
services.AddDbContext<seed_dotnetContext>();
```

The context is based in this example in a in-process SQLite database. 
The database is encrypted using SQLcipher so only the application can access. 
The key is located in the application settings file. If you need encryption, please consider a safer method to store the key.
In order to use encryption:

1. add the following package to your application

2. add the following line to you main execution thread at startup
```c#
            SQLitePCL.Batteries_V2.Init();
```
3. Modify the context to use a connection that sets the key

```c#
protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
{
    base.OnConfiguring(optionsBuilder);
    optionsBuilder.UseSqlite(this.GetConnection());
}

private DbConnection GetConnection()
{
    SqliteConnection connection =
        new SqliteConnection(this.config["ConnectionStrings:seed_dotnetContextConnection"]);

    // each connection will use the password for unencrypt the database.
    // The following code executes the PRAGMA with two SQL Queries to prevent SQL-injection in the password
    connection.Open();
    SqliteCommand command = connection.CreateCommand();
    command.CommandText = "SELECT quote($password);";
    command.Parameters.AddWithValue("$password", this.GetPassword());
    string quotedPassword = (string)command.ExecuteScalar();
    command.CommandText = "PRAGMA key = " + quotedPassword;
    command.Parameters.Clear();
    command.ExecuteNonQuery();

    return connection;
}
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
    .AddJwtBearer(cfg =>
    {
        cfg.TokenValidationParameters = new TokenValidationParameters
        {
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.config["jwt:secretKey"])),
            ValidIssuer = this.config["jwt:issuer"],
            ValidateAudience = false,
            ValidateLifetime = true
        };
    });
```        
Include the interfaces to perform the authentication
```c#
services.AddScoped<IAccountService, AccountService>();
services.AddScoped<IJwtHandler, JwtHandler>();
services.AddScoped<IPasswordHasher<UserManage>, PasswordHasher<UserManage>>();
```

### Configuration of swagger
```c#
//ConfigureServices method
 services.AddSwaggerGen(c =>
  {
     c.OperationFilter<SwaggerConsumesOperationFilter>();
     c.AddSecurityDefinition("Bearer", new ApiKeyScheme()
     {
        Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
        Name = "Authorization",
        In = "header",
        Type = "apiKey"
     });
     c.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
        {
            { "Bearer", new string[] { } }
        });

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

### Configuration of Hangfire

Hangfire is an easy way to perform background processing without any Windows process. https://www.hangfire.io/

Creation of a New Job:

Step 1: 

Include in the hangfire.contracts the interface of the new job

Step 2:

Include in Hangfire.jobs, a class to implement the interface of the new job.

To be able to execute the job include the following lines referring to the new job:

 public async Task NewJobExample(IJobCancellationToken token)
	{
		token.ThrowIfCancellationRequested();
		await NewJobExample(DateTime.Now);
	}

Step 3:
Include the job execution reference in the HangfireJobScheduler.

	RecurringJob.RemoveIfExists("New Job Name");
    RecurringJob.AddOrUpdate<NewJobExample>("New Job Name",
    job => job.NewJobExample(JobCancellationToken.Null),
    Cron.Daily, TimeZoneInfo.Local);

Step 4: 

Launch the application and access to https://{domain}/hangfire

For more information access to https://docs.hangfire.io/en/latest/

### Versioning of the API

The solution has included Microsoft.AspNetCore.Mvc.Versioning to manage the versions of the API.

For each controller you can define the default API version setting the following decorator:
[ApiVersion("1")]

If you manage different versions in the same controller you can define to which API version is related the endpoint setting the following decorator:
[MapToApiVersion("2")]


### The model mapper

Automapper is used to map the Model to the View Model and viceversa. 
A specific class is taking care to the mapping. That allows the same map to be used in the Unit Test and the application

```c#
//Map the view model object with the internal model
  Mapper.Initialize(config =>
  {
      config.CreateMap<PatientViewModel, Patient>().ReverseMap();
      config.CreateMap<UserViewModel, UserManage>().ReverseMap();
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

## Paged List

The component PagedList (https://www.nuget.org/packages/PagedList.Core/) is used to retrieve the paged list of patients. This component uses a one-based index but in the API requires a zero-based index. Thus, additions and substractions are coded to adapt both requirements

The object returned is like follows:


## DB

The seed have a integrated SQLite database, where the information of the users and the patients will stored. This database is intended only for this seed project. In your future projects you can use the database you want.

These are the changes that you need to do to change the database:

- In the appsettings.json file set the new database connection string

```json
  "ConnectionStrings": {
    "seed_dotnetContextConnection": "Data Source=.\\db\\seed_dotnetdb.db;"
  },
 ```
 
 - In the SeedDotnetContext.cs change the database property of the option builder for the correct one
 
 ```c#
    optionsBuilder.UseSqlite(_config["ConnectionStrings:seed_dotnetContextConnection"]);
 ```
 
## Database migrations

### FluentMigrator

The seed has implemented a new functionality of migrations of database schemas. A NuGet package called **FluentMigrator** is used, you can read the documentation here (https://fluentmigrator.github.io/api/index.html).

- You can easily change the database SQLite to SQLServer engine by changing the **DatabaseMigratorRunner.cs** file. 
Change the following line:
	
```c#
	.ConfigureRunner(configureRunner => configureRunner.AddSQLite()
```
for:
```c#
	.ConfigureRunner(configureRunner => configureRunner.AddSqlServer()
```

*Note:* All the most common databases are supported.


