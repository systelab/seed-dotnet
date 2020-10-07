namespace main.Contracts.Repository
{
    using main.Entities.Models;

    using X.PagedList;

    public interface IAllergyRepository : IRepositoryBase<Allergy>
    {
        IPagedList<Allergy> GetAllWithPaginationAllergy(int pageIndex, int pageSize);
    }
}