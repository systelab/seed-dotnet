namespace main.Extensions
{
    using System;
    using System.Linq;

    using AutoMapper;
    using main.Entities.Common;
    using main.Entities.Models;
    using main.Entities.ViewModels;
    using PagedList.Core;

    public class SeedMapperConfiguration : MapperConfiguration
    {
        public SeedMapperConfiguration()
            : base(
                cfg =>
                    {
                        cfg.CreateMap<AddressViewModel, Address>().ReverseMap();
                        cfg.CreateMap<PatientViewModel, Patient>()
                            .ForMember(p => p.Dob, o => o.MapFrom(q => q.Dob ?? DateTime.MinValue))
                            .ReverseMap()
                            .ForMember(p => p.Dob, o => o.MapFrom(q => (q.Dob == DateTime.MinValue) ? null : new DateTime?(q.Dob)));
                        cfg.CreateMap<UserViewModel, UserManage>().ReverseMap();
                        cfg.CreateMap<PagedList<Patient>, ExtendedPagedList<PatientViewModel>>()
                            .ForMember(p => p.TotalPages, o => o.MapFrom(q => q.PageCount))
                            .ForMember(p => p.Content, o => o.MapFrom(q => q.AsEnumerable()))
                            .ForMember(p => p.First, o => o.MapFrom(q => q.IsFirstPage))
                            .ForMember(p => p.Last, o => o.MapFrom(q => q.IsLastPage))
                            .ForMember(p => p.Size, o => o.MapFrom(q => q.PageSize))
                            .ForMember(p => p.NumberOfElements, o => o.MapFrom(q => q.Count))

                            // PagedList is a one-based index. We offer a zero-based index, therefore we have to subtract 1 to the page number
                            .ForMember(p => p.Number, o => o.MapFrom(q => q.PageNumber - 1))
                            .ForMember(p => p.TotalElements, o => o.MapFrom(q => q.TotalItemCount));
                    })
        {
        }
    }
}