using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using seed_dotnet.Models;

namespace seed_dotnet.Services
{
    /// <summary>
    /// Repository with all the queries to the database using the entity framework
    /// </summary>
    public class Seed_dotnetRepository : ISeed_dotnetRepository
    {

        private seed_dotnetContext _context;

        public IEnumerable<Patient> Patients { get; private set; }

        /// <summary>
        /// Set the context of the app
        /// </summary>
        /// <param name="context"></param>
        public Seed_dotnetRepository(seed_dotnetContext context)
        {
            _context = context;
        }
        
        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync()) > 0;
        }
        /// <summary>
        /// Insert the patient into the database
        /// </summary>
        /// <param name="newpatient">Object with the information of the patient that you want to insert</param>
        public void AddPatient(Patient newpatient)
        {
            _context.Add(newpatient);
        }
        /// <summary>
        /// Remove the patient from the database
        /// </summary>
        /// <param name="nPatient">Object with the information of the patient that you want to remove</param>
        public void DeletePatient(Patient nPatient)
        {
            _context.Entry(nPatient).State = Microsoft.EntityFrameworkCore.EntityState.Deleted;
            _context.SaveChanges();
        }
        /// <summary>
        /// List all the patients saved in the database
        /// </summary>
        /// <returns>List of patients object</returns>
        public IEnumerable<Patient> GetAllPatients()
        {
            return _context.Patients.ToList();
        }
        /// <summary>
        /// Get a specific patient
        /// </summary>
        /// <param name="nPatient">Object of the patient that you want to retrieve, in this case the id of the patient must be filled</param>
        /// <returns></returns>
        public Patient GetPatient(Patient nPatient)
        {
            return _context.Patients.Where(t => t.id == nPatient.id).FirstOrDefault();
        }
        /// <summary>
        /// Update information of the patient
        /// </summary>
        /// <param name="nPatient">Object of the patient that you want to update, they ID must be filled and the information that you want to change</param>
        public void UpdatePatient(Patient nPatient)
        {
            _context.Entry(nPatient).State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            _context.SaveChanges();
        }

       
    }
}
