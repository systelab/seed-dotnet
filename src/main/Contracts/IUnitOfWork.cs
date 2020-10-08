namespace Main.Contracts
{
    using System;

    using Main.Contracts.Repository;

    public interface IUnitOfWork : IDisposable
    {
        IAllergyRepository Allergies { get; }

        IPatientRepository Patients { get; }

        int Complete();
    }
}