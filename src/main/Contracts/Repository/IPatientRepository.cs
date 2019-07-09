namespace main.Contracts.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Entities.Models;
    using Entities.Models.Relations;
    using PagedList.Core;

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