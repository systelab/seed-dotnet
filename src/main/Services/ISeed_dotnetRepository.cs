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
        Task AddPatient(Patient newPatient);

        Task DeletePatient(Patient patient);

        Task<PagedList<Patient>> GetAllPatients(int pageNumber, int elementsPerPage);

        Task<Patient> GetPatient(Patient patient);

        Task UpdatePatient(Patient patient);

        Task<UserManage> GetUserManageWithRefreshToken(string token);
        
        Task UpdateRefreshToken(UserManage user);
    }
}