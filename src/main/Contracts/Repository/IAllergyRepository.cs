
using main.Entities.Models;
using PagedList.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace main.Contracts.Repository
{
    public interface IAllergyRepository : IRepositoryBase<Allergy>
    {
        PagedList<Allergy> GetAllWithPaginationAllergy(int pageIndex, int pageSize);
    }
}
