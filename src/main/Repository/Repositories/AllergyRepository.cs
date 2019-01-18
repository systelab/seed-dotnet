using main.Contracts.Repository;
using main.Entities;
using main.Entities.Models;
using Microsoft.EntityFrameworkCore;
using PagedList.Core;
using System;
using System.Linq;

namespace main.Repository.Repositories
{
    public class AllergyRepository : RepositoryBase<Allergy>, IAllergyRepository
    {
        public AllergyRepository(SeedDotnetContext context)
            : base(context)
        {
        }


        public PagedList<Allergy> GetAllWithPaginationAllergy(int pageIndex, int pageSize)
        {
            return new PagedList<Allergy>(context.Allergies
                .OrderBy(p => p.Name), pageIndex, pageSize);
        }

    }
}
