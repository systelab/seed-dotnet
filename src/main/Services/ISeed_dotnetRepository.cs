namespace Main.Services
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Main.Models;
    using PagedList.Core;

    /// <summary>
    /// Interface with all the method needed
    /// </summary>
    public interface ISeedDotnetRepository
    {
        void AddPatient(Patient nPatient);

        List<Patient> DeletePatient(Patient nPatient);

        List<Patient> GetAllPatients();

        PagedList<Patient> GetAllPatients(int pageNumber, int elementsPerPage);

        Patient GetPatient(Patient nPatient);

        Task<bool> SaveChangesAsync();

        void UpdatePatient(Patient nPatient);

        UserManage GetUserManageWithRefreshToken(string token);
        void UpdateRefreshToken(UserManage user);
    }
}