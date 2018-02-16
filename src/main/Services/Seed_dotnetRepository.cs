namespace Main.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Main.Models;

    /// <summary>
    /// Repository with all the queries to the database using the entity framework
    /// </summary>
    public class SeedDotnetRepository : ISeedDotnetRepository
    {
        private readonly SeedDotnetContext context;

        /// <summary>
        /// Set the context of the app
        /// </summary>
        /// <param name="_context"></param>
        public SeedDotnetRepository(SeedDotnetContext _context)
        {
            this.context = _context;
        }

        public List<Patient> Patients { get; private set; }

        /// <summary>
        /// Insert the patient into the database
        /// </summary>
        /// <param name="newpatient">Object with the information of the patient that you want to insert</param>
        public void AddPatient(Patient newpatient)
        {
            this.context.Add(newpatient);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove the patient from the database
        /// </summary>
        /// <param name="nPatient">Object with the information of the patient that you want to remove</param>
        public List<Patient> DeletePatient(Patient nPatient)
        {
            this.context.Entry(nPatient).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            this.context.SaveChanges();
            return this.context.Patients.ToList();
        }

        /// <summary>
        /// List all the patients saved in the database
        /// </summary>
        /// <returns>List of patients object</returns>
        public List<Patient> GetAllPatients()
        {
            return this.context.Patients.ToList();
        }

        /// <summary>
        /// Get a specific patient
        /// </summary>
        /// <param name="nPatient">Object of the patient that you want to retrieve, in this case the id of the patient must be filled</param>
        /// <returns></returns>
        public Patient GetPatient(Patient nPatient)
        {
            return this.context.Patients.Where(t => t.Id == nPatient.Id).FirstOrDefault();
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await this.context.SaveChangesAsync()) > 0;
        }

        /// <summary>
        /// Update information of the patient
        /// </summary>
        /// <param name="nPatient">Object of the patient that you want to update, they ID must be filled and the information that you want to change</param>
        public void UpdatePatient(Patient nPatient)
        {
            this.context.Entry(nPatient).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();
        }
        /// <summary>
        /// Get the user information providing a refresh token, in this case we are using a database but you can use other system.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public UserManage GetUserManageWithRefreshToken(string token)
        {
            return this.context.Users.Where(t => t.RefreshToken == token).FirstOrDefault();
        }
        /// <summary>
        /// Update the refresh token of the user session
        /// </summary>
        /// <param name="user"></param>
        public void UpdateRefreshToken(UserManage user)
        {
            this.context.Entry(user).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            this.context.SaveChanges();
        }
    }

}