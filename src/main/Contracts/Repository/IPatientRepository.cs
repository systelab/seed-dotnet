﻿namespace main.Contracts.Repository
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using main.Entities.Models;
    using main.Entities.Models.Relations;

    using X.PagedList;

    public interface IPatientRepository : IRepositoryBase<Patient>
    {
        Task<PatientAllergy> AddAllergy(PatientAllergy patientAllergy);

        Task<IList<PatientAllergy>> GetAllergies(Guid idPatient);

        Task<IPagedList<Patient>> GetAllWithPaginationPatients(int pageIndex, int pageSize);

        Task<PatientAllergy> GetPatientAllergy(Guid idPatient, Guid idAllergy);

        Task RemoveAllergy(Guid idPatient, Guid idAllergy);

        Task UpdatePatientAllergy(PatientAllergy patientAllergy);
    }
}