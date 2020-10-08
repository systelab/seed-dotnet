namespace Main.Repository.Repositories
{
    using System.Linq;

    using Main.Contracts.Repository;
    using Main.Entities;
    using Main.Entities.Models;

    using X.PagedList;

    public class AllergyRepository : RepositoryBase<Allergy>, IAllergyRepository
    {
        public AllergyRepository(SeedDotnetContext context)
            : base(context)
        {
        }

        public IPagedList<Allergy> GetAllWithPaginationAllergy(int pageIndex, int pageSize)
        {
            return new PagedList<Allergy>(this.context.Allergies.OrderBy(p => p.Name), pageIndex, pageSize);
        }
    }
}