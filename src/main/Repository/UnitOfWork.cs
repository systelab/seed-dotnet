namespace main.Repository
{
    using main.Contracts;
    using main.Contracts.Repository;
    using main.Entities;
    using main.Repository.Repositories;

    /// <summary>
    /// 
    /// </summary>
    public class UnitOfWork : IUnitOfWork
    {
        private readonly SeedDotnetContext context;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="context"></param>
        public UnitOfWork(SeedDotnetContext context)
        {
            this.context = context;
            this.Patients = new PatientRepository(this.context);
            this.Allergies = new AllergyRepository(this.context);
        }

        public IAllergyRepository Allergies { get; }

        public IPatientRepository Patients { get; }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int Complete()
        {
            return this.context.SaveChanges();
        }

        /// <summary>
        /// 
        /// </summary>
        public void Dispose()
        {
            this.context.Dispose();
            this.context.CloseConnection();
        }
    }
}