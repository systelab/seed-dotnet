namespace Main.Contracts
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Main.Entities.Models;
    using Main.Entities.Models.Relations;

    using X.PagedList;

    /// <summary>
    /// Service to manage patient related information
    /// </summary>
    public interface IPatientService
    {
        Task<PatientAllergy> AddAllergy(PatientAllergy allergy, Guid uid);

        Task<Patient> Create(Patient patient);

        Task Delete(Guid uid);

        Task<IPagedList<Patient>> Get(int page, int elementsPerPage);

        Task<Patient> Get(Guid patientUid);

        Task<IList<PatientAllergy>> GetAllergies(Guid uid);

        Task RemoveAllergy(Guid uid, Guid uidAllergy);

        Task<Patient> Update(Guid uid, Patient patient);

        Task<PatientAllergy> UpdateAllergy(Guid uid, Guid uidAllergy, PatientAllergy allergy);
    }
}