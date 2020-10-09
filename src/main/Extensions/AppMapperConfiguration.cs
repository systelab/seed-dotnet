namespace Main.Extensions
{
    using System;
    using System.Linq;

    using AutoMapper;

    using Main.Entities.Common;
    using Main.Entities.Models;
    using Main.Entities.ViewModels;

    using X.PagedList;

    public class AppMapperConfiguration : Profile
    {
        public AppMapperConfiguration()
        {
            CreateMap<AddressViewModel, Address>().ForMember(p => p.CreationTime, o => o.Ignore())
                .ForMember(p => p.UpdateTime, o => o.Ignore()).ReverseMap();
            CreateMap<PatientViewModel, Patient>().ForMember(p => p.CreationTime, o => o.Ignore())
                .ForMember(p => p.UpdateTime, o => o.Ignore()).ForMember(p => p.Dob, o => o.MapFrom(q => q.Dob ?? DateTime.MinValue)).ReverseMap().ForMember(
                p => p.Dob,
                o => o.MapFrom(q => q.Dob == DateTime.MinValue ? null : new DateTime?(q.Dob)));
            CreateMap<UserViewModel, UserManage>()
                .ForMember(p => p.LastName, o => o.MapFrom(q => q.LastName))
                .ForMember(p => p.Name, o => o.MapFrom(q => q.Name))
                .ForMember(p => p.Email, o => o.MapFrom(q => q.Email))
                .ForAllOtherMembers(o => o.Ignore());
            CreateMap<UserManage, UserViewModel>();
        }
    }
}