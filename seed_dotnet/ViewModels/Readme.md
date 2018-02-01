# Seed-dotnet ViewModels

The view models are the models which the controllers use to response the information to requested by the API requests and the models requested in the API requests.

The view models can be a combinations or more than one internal model.

We use Automapper to translate a internal model to a external model and 

```c#
  var results = _repository.GetAllPatients();
  return Ok(Mapper.Map<IEnumerable<PatientViewModel>>(results));
 ```
