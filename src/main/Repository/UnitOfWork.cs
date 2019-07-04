namespace main.Repository
{
    using Contracts;
    using Contracts.Repository;
    using Entities;
    using Repositories;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly SeedDotnetContext _context;

        public UnitOfWork(SeedDotnetContext context)
        {
            this._context = context;
            this.Patients = new PatientRepository(this._context);
            this.Allergies = new AllergyRepository(this._context);
        }

        public IPatientRepository Patients { get; }
        public IAllergyRepository Allergies { get; }


        public int Complete()
        {
            return this._context.SaveChanges();
        }

        public void Dispose()
        {
            this._context.Dispose();
            this._context.CloseConnection();
        }
    }
}