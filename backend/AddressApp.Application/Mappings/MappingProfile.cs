using AutoMapper;
using AddressApp.Application.DTOs;
using AddressApp.Core.Entities;

namespace AddressApp.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Address, AddressDto>();
        CreateMap<CreateAddressDto, Address>();
        CreateMap<UpdateAddressDto, Address>();
    }
}