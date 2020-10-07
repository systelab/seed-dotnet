# Seed-dotnet Repositories

The role of the Repository is to access data, wherever is the data and in whatever format it could be. For the external world (the domain), the repository must deal in Domain Entities, making the rest of the system decoupled from its data source.

In this folder we can found the Interface of the repository [ISeed_dotnetRepository] and a implementation of the repository [Seed_donetRepository]. The methods implemented are requested by the controllers and the repository is in charge to request the information to the database and made the logic needed to prepare and response the data to the controllers.

Also we can find the interface to create a new JWT token [IJwtHandler] and the interface to procced with the login and the token refresh [IAccountService].


[Seed_donetRepository]:https://github.com/systelab/seed-dotnet/blob/master/seed_dotnet/Services/Seed_dotnetRepository.cs
[ISeed_dotnetRepository]:https://github.com/systelab/seed-dotnet/blob/master/seed_dotnet/Services/ISeed_dotnetRepository.cs
[IAccountService]:https://github.com/systelab/seed-dotnet/blob/master/src/main/Services/IAccountService.cs
[IJwtHandler]:https://github.com/systelab/seed-dotnet/blob/master/src/main/Services/IJwtHandler.cs

## MailService

The mail service contains the methods to create the email template an sending the email.

