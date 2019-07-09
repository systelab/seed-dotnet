namespace main.Repository.Repositories
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Contracts.Repository;
    using Entities;
    using Entities.Models;
    using Entities.Models.Relations;
    using Microsoft.EntityFrameworkCore;
    using PagedList.Core;

    public class PatientRepository : RepositoryBase<Patient>, IPatientRepository
    {
        public PatientRepository(SeedDotnetContext context)
            : base(context)
        {
        }

        public bool AddAllergy(PatientAllergy patientAllergy)
        {
            this.context.PatientAllergies.Add(patientAllergy);
            this.context.SaveChanges();
            return true;
        }

        public bool RemoveAllergy(Guid idPatient, Guid idAllergy)
        {
            PatientAllergy pa = this.context.PatientAllergies.Where(p => p.IdAllergy == idAllergy)
                .Where(a => a.IdPatient == idPatient).First();
            this.context.PatientAllergies.Remove(pa);
            this.context.SaveChanges();
            return true;
        }

        public List<PatientAllergy> GetAllergies(Guid idPatient)
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
            return q.ToList();
        }

        public async Task<PagedList<Patient>> GetAllWithPaginationPatients(int pageIndex, int pageSize)
        {
            return await Task.Run(() => new PagedList<Patient>(this.context.Patients
                .Include(c => c.Address)
                .OrderBy(p => p.Name), pageIndex, pageSize));
        }

        public PatientAllergy GetPatientAllergy(Guid idPatient, Guid idAllergy)
        {
            if (this.context.PatientAllergies.Where(p => p.IdAllergy == idAllergy).Where(a => a.IdPatient == idPatient)
                    .Count() > 0)
            {
                return this.context.PatientAllergies.Where(p => p.IdAllergy == idAllergy)
                    .Where(a => a.IdPatient == idPatient).First();
            }

            return null;
        }

        public bool UpdatePatientAllergy(PatientAllergy patientAllergy)
        {
            this.context.PatientAllergies.Update(patientAllergy);
            this.context.SaveChanges();
            return true;
        }
    }
}