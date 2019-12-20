# Logging

There are three main players regarding logging in .net:

- log4net (implementation of a logger)
- Nlog (implementation of a logger)
- Microsoft.Extensions.Logging (specially for ASP.NET)

[log4net](https://logging.apache.org/log4net/) is well backed-up but poorly documented, plenty of examples and questions can be found on the internet, although.

[NLog](https://nlog-project.org/) is simpler than log4net and also achieves good results. Either of them is a good choice

The [Microsoft Extension](https://blog.stephencleary.com/2018/05/microsoft-extensions-logging-part-1-introduction.html) is a DI approach acting as a wrapper for third-party providers and built-in loggers. 

Be aware that some [authors](https://stackoverflow.com/questions/41243485/simple-injector-register-iloggert-by-using-iloggerfactory-createloggert/41244169#41244169) believe that the whole Microsoft.Extensions.Logging design is a violation of the Dependency Injection Principle, with the ILogger<T> specifically a violation of the Interface Segregation Principle. 

The seed in this case is using Microsoft.Extensions.Logging with a third-party that is NLog, just to show the power of abstractions.

### Logging with Nlog

Follow the [instructions] (https://github.com/NLog/NLog/wiki/Getting-started-with-ASP.NET-Core-2) to add NLog to your project.

Configure the [extensions](https://nlog-project.org/config/) you want.

ASPNET already uses a DI container approach and therefore all controllers may receive an instance of `ILogger<T>` as a parameter
```
public PatientController(IUnitOfWork unitOfWork, ILogger<PatientController> logger, IMapper mapper, IMedicalRecordNumberService medicalRecordNumberService)
```

Using this `logger` inside the controller will be using at the end the NLog implementation as configured at the startup.
