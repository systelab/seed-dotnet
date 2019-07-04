namespace main.Contracts
{
    using System;
    using Repository;

    public interface IUnitOfWork : IDisposable
    {
        /*Referencia a las interfaces */
        IPatientRepository Patients { get; }
        IAllergyRepository Allergies { get; }

        int Complete();
    }
}