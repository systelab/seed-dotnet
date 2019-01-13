using main.Contracts.Repository;
using System;


namespace main.Contracts
{
    public interface IUnitOfWork : IDisposable
    {
        /*Referencia a las interfaces */
        IPatientRepository Patients { get; }
        IAllergyRepository Allergies { get; }

        int Complete();
    }
}
