using AutoMapper;
using AddressApp.Application.DTOs;
using AddressApp.Domain.Entities;

namespace AddressApp.Application.Mappings
{
    public class AddressProfile : Profile
    {
        public AddressProfile()
        {
            CreateMap<Address, AddressDto>();
            CreateMap<CreateAddressDto, Address>();
            CreateMap<UpdateAddressDto, Address>();
        }
    }
}