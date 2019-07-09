namespace main.Repository.Repositories
{
    using System.Linq;
    using Contracts.Repository;
    using Entities;
    using Entities.Models;
    using PagedList.Core;

    public class AllergyRepository : RepositoryBase<Allergy>, IAllergyRepository
    {
        public AllergyRepository(SeedDotnetContext context)
            : base(context)
        {
        }

        public PagedList<Allergy> GetAllWithPaginationAllergy(int pageIndex, int pageSize)
        {
            return new PagedList<Allergy>(this.context.Allergies
                .OrderBy(p => p.Name), pageIndex, pageSize);
        }
    }
}