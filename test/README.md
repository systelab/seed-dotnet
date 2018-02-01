# Seed_dotnet Test

## Unit Test

The testing is focused in testing the controllers, in this case the controller tested is [PatientController].

The test are examples or how you can test a controller.

For example: 

- GetAllPatients_Should_Return_List_Of_Patients (Possitive Case)
- GetPatients_Should_Return_Patient_Information (Possitive Case)
- CreatePatient_ReturnsBadRequest_GivenInvalidPatient (Negative Case)
- CreatePatient_Should_Create_A_New_Patient (Possitive Case)
- RemovePatient_Should_Remove_A_Existing_Patient (Possitive Case)

The check of the results we done using xUnit
```c#
Xunit.Assert.IsType<OkObjectResult>(result.Result);

//or

Xunit.Assert.Equal(3, model.Count());
```
To generate the mock we use Moq
```c#
 _mockUserRepo.Setup(repo => repo.DeletePatient(It.IsAny<Patient>())).Returns(lpatient);
 
```

To have fake data to perfom the test, there is a Test Initialization method where create a list of fake patients in the lpatient variable.


## Integration Test

We are working to have this ready as soon as possible


[PatientController]: https://github.com/systelab/seed-dotnet/blob/master/main/Controllers/Api/PatientController.cs
