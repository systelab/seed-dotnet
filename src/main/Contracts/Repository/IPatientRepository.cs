
using main.Entities.Models;
using main.Entities.Models.Relations;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace main.Contracts.Repository
{
    public interface IPatientRepository : IRepositoryBase<Patient>
    {
        Task<PagedList<Patient>> GetAllWithPaginationPatients(int pageIndex, int pageSize);

        bool AddAllergy(PatientAllergy patientAllergy);
        List<PatientAllergy> GetAllergies(Guid idPatient);
        bool RemoveAllergy(Guid idPatient, Guid idAllergy);
        PatientAllergy GetPatientAllergy(Guid idPatient, Guid idAllergy);
        bool UpdatePatientAllergy(PatientAllergy patientAllergy);
    }
}
