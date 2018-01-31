# Seed-dontet Controllers


As a controller in this seed we can find the controller in charge of the session management(validate the user and generate the token) and the controller in charge of the CRUD of a Patient.

## AuthController

There are two REST points:

### /users/login

Is a post [HttpPost] REST end point.

As a parameter request a [LoginViewModel] object.

As a result if the login is successfull returns a JWT Token.


### /users/
Is a get [HttpGet] REST end point, and in this case the authentication token is needed.

Return the information of the logged user.

[LoginViewModel]: https://github.com/systelab/seed-dotnet/blob/master/seed_dotnet/ViewModels/LoginViewModel.cs
