namespace seed_dotnet.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using seed_dotnet.Models;

    /// <summary>
    /// Interface with all the method needed
    /// </summary>
    public interface ISeed_dotnetRepository
    {
        void AddPatient(Patient nPatient);

        List<Patient> DeletePatient(Patient nPatient);

        List<Patient> GetAllPatients();

        Patient GetPatient(Patient nPatient);

        Task<bool> SaveChangesAsync();

        void UpdatePatient(Patient nPatient);
    }
}