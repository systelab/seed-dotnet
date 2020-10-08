namespace Main.Repository.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using Main.Contracts.Repository;
    using Main.Entities;
    using Main.Entities.Models;
    using Main.Entities.Models.Relations;

    using Microsoft.EntityFrameworkCore;

    using X.PagedList;

    public class PatientRepository : RepositoryBase<Patient>, IPatientRepository
    {
        public PatientRepository(DatabaseContext context)
            : base(context)
        {
        }

        public async Task<PatientAllergy> AddAllergy(PatientAllergy patientAllergy)
        {
            this.context.PatientAllergies.Add(patientAllergy);
            await this.context.SaveChangesAsync();
            return patientAllergy;
        }

        public async Task<IList<PatientAllergy>> GetAllergies(Guid idPatient)
        {
            IQueryable<PatientAllergy> q = from c in this.context.PatientAllergies
                                           join r in this.context.Allergies on c.IdAllergy equals r.Id
                                           where c.IdPatient == idPatient
                                           orderby r.Name
                                           select new PatientAllergy
                                                      {
                                                          Id = c.Id,
                                                          IdPatient = c.IdPatient,
                                                          IdAllergy = c.IdAllergy,
                                                          Note = c.Note,
                                                          LastOcurrence = c.LastOcurrence,
                                                          AssertedDate = c.AssertedDate,
                                                          Allergy = r
                                                      };
            return await q.ToListAsync();
        }

        public async Task<IPagedList<Patient>> GetAllWithPaginationPatients(int pageIndex, int pageSize)
        {
            return await this.context.Patients.Include(c => c.Address).OrderBy(p => p.Name).ToPagedListAsync(pageIndex, pageSize);
        }

        public async Task<PatientAllergy> GetPatientAllergy(Guid idPatient, Guid idAllergy)
        {
            return await this.context.PatientAllergies.FirstOrDefaultAsync(p => p.IdAllergy == idAllergy && p.IdPatient == idPatient);
        }

        public async Task RemoveAllergy(Guid idPatient, Guid idAllergy)
        {
            PatientAllergy pa = await this.context.PatientAllergies.FirstOrDefaultAsync(p => p.IdAllergy == idAllergy && p.IdPatient == idPatient);
            this.context.PatientAllergies.Remove(pa);
            await this.context.SaveChangesAsync();
        }

        public async Task UpdatePatientAllergy(PatientAllergy patientAllergy)
        {
            this.context.PatientAllergies.Update(patientAllergy);
            await this.context.SaveChangesAsync();
        }
    }
}