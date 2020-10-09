namespace Main.Services
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using Main.Contracts;
    using Main.Entities.Models;
    using Main.Entities.Models.Relations;

    using X.PagedList;

    public class PatientService : IPatientService
    {
        private readonly IMedicalRecordNumberService medicalRecordNumberService;

        private readonly IUnitOfWork unitOfWork;

        public PatientService(IMedicalRecordNumberService medicalRecordNumberService, IUnitOfWork unitOfWork)
        {
            this.medicalRecordNumberService = medicalRecordNumberService;
            this.unitOfWork = unitOfWork;
        }

        public async Task<PatientAllergy> AddAllergy(PatientAllergy patientAllergy, Guid uid)
        {
            return await this.unitOfWork.Patients.AddAllergy(patientAllergy);
        }

        public async Task<Patient> Create(Patient patient)
        {
            patient.MedicalNumber = this.medicalRecordNumberService.GetMedicalRecordNumber("http://localhost:9090");
            if (patient.Id.Equals(Guid.Empty))
            {
                patient.Id = Guid.NewGuid();
            }

            await this.unitOfWork.Patients.Add(patient);
            return patient;
        }

        public async Task Delete(Guid uid)
        {
            Patient patientToDelete = await this.unitOfWork.Patients.Get(uid);
            if (patientToDelete == null)
            {
                throw new PatientNotFoundException();
            }

            await this.unitOfWork.Patients.Remove(patientToDelete);
        }

        public async Task<IPagedList<Patient>> Get(int page, int elementsPerPage)
        {
            return await this.unitOfWork.Patients.GetAllWithPaginationPatients(page, elementsPerPage);
        }

        public async Task<Patient> Get(Guid id)
        {
            return await this.unitOfWork.Patients.Get(id);
        }

        public async Task<IList<PatientAllergy>> GetAllergies(Guid uid)
        {
            return await this.unitOfWork.Patients.GetAllergies(uid);
        }

        public async Task RemoveAllergy(Guid uid, Guid uidAllergy)
        {
            await this.unitOfWork.Patients.RemoveAllergy(uid, uidAllergy);
        }

        public async Task<Patient> Update(Guid uid, Patient updatedPatient)
        {
            Patient results = await this.unitOfWork.Patients.Get(uid);

            if (results == null)
            {
                throw new PatientNotFoundException();
            }

            if (!string.IsNullOrWhiteSpace(updatedPatient.Name))
            {
                results.Name = updatedPatient.Name;
            }

            if (!string.IsNullOrWhiteSpace(updatedPatient.Email))
            {
                results.Email = updatedPatient.Email;
            }

            if (!string.IsNullOrWhiteSpace(updatedPatient.Surname))
            {
                results.Surname = updatedPatient.Surname;
            }

            if (!string.IsNullOrWhiteSpace(updatedPatient.MedicalNumber))
            {
                results.MedicalNumber = updatedPatient.MedicalNumber;
            }

            await this.unitOfWork.Patients.Update(results);

            return results;
        }

        public async Task<PatientAllergy> UpdateAllergy(Guid uid, Guid uidAllergy, PatientAllergy allergy)
        {
            PatientAllergy patientAllergy = await this.unitOfWork.Patients.GetPatientAllergy(uid, uidAllergy);
            if (patientAllergy == null || patientAllergy.Id.Equals(Guid.Empty))
            {
                throw new PatientAllergyNotFoundException();
            }

            if (!string.IsNullOrWhiteSpace(patientAllergy.Note))
            {
                patientAllergy.Note = patientAllergy.Note;
            }

            if (patientAllergy.LastOcurrence != DateTime.MinValue)
            {
                patientAllergy.LastOcurrence = patientAllergy.LastOcurrence;
            }

            if (patientAllergy.AssertedDate != DateTime.MinValue)
            {
                patientAllergy.AssertedDate = patientAllergy.AssertedDate;
            }

            this.unitOfWork.Patients.UpdatePatientAllergy(patientAllergy);

            return patientAllergy;
        }
    }
}