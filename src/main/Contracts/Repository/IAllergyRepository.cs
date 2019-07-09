namespace main.Contracts.Repository
{
    using Entities.Models;
    using PagedList.Core;

    public interface IAllergyRepository : IRepositoryBase<Allergy>
    {
        PagedList<Allergy> GetAllWithPaginationAllergy(int pageIndex, int pageSize);
    }
}