using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using seed_dotnet.Models;

namespace seed_dotnet.Services
{
    /// <summary>
    /// Interface with all the method needed
    /// </summary>
    public interface ISeed_dotnetRepository
    {
       
        Task<bool> SaveChangesAsync();
        List<Patient> GetAllPatients();
        Patient GetPatient(Patient nPatient);
        void AddPatient(Patient nPatient);
        List<Patient> DeletePatient(Patient nPatient);
        void UpdatePatient(Patient nPatient);

    }
}
