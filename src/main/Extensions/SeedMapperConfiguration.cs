namespace Main.Extensions
{
    using System;
    using System.Linq;

    using AutoMapper;

    using Main.Entities.Common;
    using Main.Entities.Models;
    using Main.Entities.ViewModels;

    using X.PagedList;

    public class SeedMapperConfiguration : MapperConfiguration
    {
        public SeedMapperConfiguration()
            : base(
                cfg =>
                    {
                        cfg.CreateMap<AddressViewModel, Address>().ForMember(p => p.CreationTime, o => o.Ignore())
                            .ForMember(p => p.UpdateTime, o => o.Ignore()).ReverseMap();
                        cfg.CreateMap<PatientViewModel, Patient>().ForMember(p => p.CreationTime, o => o.Ignore())
                            .ForMember(p => p.UpdateTime, o => o.Ignore()).ForMember(p => p.Dob, o => o.MapFrom(q => q.Dob ?? DateTime.MinValue)).ReverseMap().ForMember(
                            p => p.Dob,
                            o => o.MapFrom(q => q.Dob == DateTime.MinValue ? null : new DateTime?(q.Dob)));
                        cfg.CreateMap<UserViewModel, UserManage>()
                            .ForMember(p => p.LastName, o => o.MapFrom(q => q.LastName))
                            .ForMember(p => p.Name, o => o.MapFrom(q => q.Name))
                            .ForMember(p => p.Email, o => o.MapFrom(q => q.Email))
                            .ForAllOtherMembers(o => o.Ignore());
                        cfg.CreateMap<UserManage, UserViewModel>();
                        cfg.CreateMap<IPagedList<Patient>, ExtendedPagedList<PatientViewModel>>().ForMember(p => p.TotalPages, o => o.MapFrom(q => q.PageCount))
                            .ForMember(p => p.Content, o => o.MapFrom(q => q.AsEnumerable())).ForMember(p => p.First, o => o.MapFrom(q => q.IsFirstPage))
                            .ForMember(p => p.Last, o => o.MapFrom(q => q.IsLastPage)).ForMember(p => p.Size, o => o.MapFrom(q => q.PageSize))
                            .ForMember(p => p.NumberOfElements, o => o.MapFrom(q => q.Count))

                            // PagedList is a one-based index. We offer a zero-based index, therefore we have to subtract 1 to the page number
                            .ForMember(p => p.Number, o => o.MapFrom(q => q.PageNumber - 1)).ForMember(p => p.TotalElements, o => o.MapFrom(q => q.TotalItemCount));
                    })
        {
        }
    }
}