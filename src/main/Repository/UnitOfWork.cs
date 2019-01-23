using main.Contracts;
using main.Contracts.Repository;
using main.Entities;
using main.Repository.Repositories;


namespace main.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SeedDotnetContext _context;

        public UnitOfWork(SeedDotnetContext context)
        {
            _context = context;
            Patients = new PatientRepository(_context);
            Allergies = new AllergyRepository(_context);

        }

        public IPatientRepository Patients { get; private set; }
        public IAllergyRepository Allergies { get; private set; }


        public int Complete()
        {
            return _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
            _context.CloseConnection();
        }
    }
}
