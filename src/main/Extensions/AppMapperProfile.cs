namespace Main.Extensions
{
    using System;

    using AutoMapper;

    using Main.Entities.Models;
    using Main.Entities.ViewModels;

    public class AppMapperProfile : Profile
    {
        public AppMapperProfile()
        {
            this.CreateMap<AddressViewModel, Address>().ForMember(p => p.CreationTime, o => o.Ignore()).ForMember(p => p.UpdateTime, o => o.Ignore()).ReverseMap();
            this.CreateMap<PatientViewModel, Patient>().ForMember(p => p.CreationTime, o => o.Ignore()).ForMember(p => p.UpdateTime, o => o.Ignore())
                .ForMember(p => p.Dob, o => o.MapFrom(q => q.Dob ?? DateTime.MinValue)).ReverseMap().ForMember(
                    p => p.Dob,
                    o => o.MapFrom(q => q.Dob == DateTime.MinValue ? null : new DateTime?(q.Dob)));
            this.CreateMap<UserViewModel, UserManage>().ForMember(p => p.LastName, o => o.MapFrom(q => q.LastName)).ForMember(p => p.Name, o => o.MapFrom(q => q.Name))
                .ForMember(p => p.Email, o => o.MapFrom(q => q.Email)).ForAllOtherMembers(o => o.Ignore());
            this.CreateMap<UserManage, UserViewModel>();
        }
    }
}