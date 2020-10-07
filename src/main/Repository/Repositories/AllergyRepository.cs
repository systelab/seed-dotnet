namespace main.Repository.Repositories
{
    using System.Linq;

    using main.Contracts.Repository;
    using main.Entities;
    using main.Entities.Models;

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