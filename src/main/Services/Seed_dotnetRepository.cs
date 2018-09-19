namespace Main.Services
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Main.Models;

    using Microsoft.EntityFrameworkCore;

    using PagedList.Core;

    /// <summary>
    /// Repository with all the queries to the database using the entity framework
    /// </summary>
    internal class SeedDotnetRepository : ISeedDotnetRepository
    {
        private readonly SeedDotnetContext context;

        /// <summary>
        /// Initializes a new instance of the <see cref="SeedDotnetRepository"/> class. 
        /// </summary>
        /// <param name="context">
        /// database context
        /// </param>
        public SeedDotnetRepository(SeedDotnetContext context)
        {
            this.context = context;
        }

        /// <summary>
        /// Insert the patient into the database
        /// </summary>
        /// <param name="newPatient">
        /// Object with the information of the patient that you want to insert
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task AddPatient(Patient newPatient)
        {
            await this.context.AddAsync(newPatient);
            this.context.SaveChanges();
        }

        /// <summary>
        /// Remove the patient from the database
        /// </summary>
        /// <param name="patient">
        /// Object with the information of the patient that you want to remove
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task DeletePatient(Patient patient)
        {
            this.context.Entry(patient).State = EntityState.Deleted;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// List all the patients saved in the database
        /// </summary>
        /// <returns>List of patients object</returns>
        public async Task<List<Patient>> GetAllPatients()
        {
            return await this.context.Patients.ToListAsync();
        }

        /// <summary>
        /// List all the patients using pagination
        /// </summary>
        /// <param name="pageNumber">
        /// The page number.
        /// </param>
        /// <param name="elementsPerPage">
        /// The elements per page.
        /// </param>
        /// <returns>
        /// The list <see cref="PagedList"/>.
        /// </returns>
        public async Task<PagedList<Patient>> GetAllPatients(int pageNumber, int elementsPerPage)
        {
            return await Task.Run(() => new PagedList<Patient>(this.context.Patients, pageNumber, elementsPerPage));
        }

        /// <summary>
        /// Get a specific patient
        /// </summary>
        /// <param name="patient">
        /// Object of the patient that you want to retrieve, in this case the id of the patient must be filled
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<Patient> GetPatient(Patient patient)
        {
            return await this.context.Patients.Where(t => t.Id == patient.Id).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Get the user information providing a refresh token, in this case we are using a database but you can use other system.
        /// </summary>
        /// <param name="token">token to refresh
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task<UserManage> GetUserManageWithRefreshToken(string token)
        {
            return await this.context.Users.Where(t => t.RefreshToken == token).FirstOrDefaultAsync();
        }

        /// <summary>
        /// Update information of the patient
        /// </summary>
        /// <param name="patient">
        /// Object of the patient that you want to update, they ID must be filled and the information that you want to change
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task UpdatePatient(Patient patient)
        {
            this.context.Entry(patient).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }

        /// <summary>
        /// Update the refresh token of the user session
        /// </summary>
        /// <param name="user">user to refresh the token
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        public async Task UpdateRefreshToken(UserManage user)
        {
            this.context.Entry(user).State = EntityState.Modified;
            await this.context.SaveChangesAsync();
        }
    }
}