using AutoMapper;
using Outlay.Infrastructure.Models;
using Outlay.Infrastructure.Models.Responses;

namespace Outlay.Infrastructure.Mapper;

public class ClientInfoProfile : Profile
{
    public ClientInfoProfile()
    {
        CreateMap<ClientInfo, ClientInfoResponse>()
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Name))
            .ForMember(x => x.CardInfos, opt => opt.MapFrom(x => x.Accounts));

        CreateMap<ClientAccount, CardInfoResponse>()
            .ForMember(x => x.Balance, opt => opt.MapFrom(x => x.Balance / 100))
            .ForMember(x => x.Id, opt => opt.MapFrom(x => x.Id))
            .ForMember(x => x.Type, opt => opt.MapFrom(x => x.Type))
            .ForMember(x => x.CurrencyCode, opt => opt.MapFrom(x => x.CurrencyCode))
            .ForMember(x => x.MaskedCardNumber, opt => opt.MapFrom(x => x.MaskedPan.FirstOrDefault()));
    }
}