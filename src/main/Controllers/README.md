# Seed-dontet Controllers


As a controller in this seed we can find the controller in charge of the session management(validate the user and generate the token) and the controller in charge of the CRUD of a Patient.

## AuthController

Contains three REST points:

### Login the user and get the session token

Is a post [HttpPost] REST end point.

As a parameter request a [LoginViewModel] object, but  in the from form not in the body.
```c#
public async Task<IActionResult> GetToken([FromForm] LoginViewModel vm)
```

As a result if the login is successfull returns a JWT Token in the Response header.

```c#
 Response.Headers.Add("Authorization", "Bearer " + result.AccessToken);
```
Also the endpoint return a refresh token in the header.
```c#
 Response.Headers.Add("Refresh", result.RefreshToken);
```
To allow the read off the header by the external system you need to add the header "Access-Control-Expose-Headers".

```c#
 Response.Headers.Add("Access-Control-Expose-Headers", "origin, content-type, accept, authorization, ETag, if-none-match");
```

### Get Logged User information
Is a get [HttpGet] REST end point, and in this case the authentication token is needed.

Return the information of the logged user.

### Check and refresh the current token

Is a [HttpPost] REST end point, this end point return if the provided refresh token is correct a new authorization token.


## PatientController

Contains the REST end points needed to implemented a CRUD of patients and for all authentification is needed.

The information returned about the patients follow the model of [PatientViewModel].

The constructor of this controller inject the dependency of the Repository Interface [ISeed_dotnetRepository], where are all the methods needed to retrive the information.
```c#
private ISeed_dotnetRepository _repository;

public PatientController(ISeed_dotnetRepository repository)
{
    _repository = repository;
}
```
[ISeed_dotnetRepository]:https://github.com/systelab/seed-dotnet/blob/master/seed_dotnet/Services/ISeed_dotnetRepository.cs

### Get Patient

Providing Patient ID return the information of the Patient.

### Get All Patients

Return a list of all the patients stored in the database.

### Create a Patient

Providing Patient information and return the information of the patient.

### Delete a Patient

Providing Patient ID, the REST End point remove the information about the patient.

### Update a Patient
Providing Patient information ,with the patient Id informed, update the information of the chosen patient.

[LoginViewModel]: https://github.com/systelab/seed-dotnet/blob/master/seed_dotnet/ViewModels/LoginViewModel.cs


[PatientViewModel]: https://github.com/systelab/seed-dotnet/blob/master/seed_dotnet/ViewModels/PatientViewModel.cs


## EmailController

Contains the REST end point needed to send an email example.

### SendEmail

Providing a model with the email of the receiver and a subject, the solution sends an example email.

[EmailViewModel]: https://github.com/systelab/seed-dotnet/blob/master/seed_dotnet/ViewModels/EmailViewModel.cs