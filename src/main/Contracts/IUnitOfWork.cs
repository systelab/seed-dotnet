namespace main.Contracts
{
    using System;

    using main.Contracts.Repository;

    public interface IUnitOfWork : IDisposable
    {
        IAllergyRepository Allergies { get; }

        /*Referencia a las interfaces */
        IPatientRepository Patients { get; }

        int Complete();
    }
}