namespace Main.Contracts.Repository
{
    using Main.Entities.Models;

    using X.PagedList;

    public interface IAllergyRepository : IRepositoryBase<Allergy>
    {
        IPagedList<Allergy> GetAllWithPaginationAllergy(int pageIndex, int pageSize);
    }
}