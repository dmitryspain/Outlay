using System.Globalization;
using AutoMapper;
using Outlay.Infrastructure.Converters;
using Outlay.Infrastructure.Models;
using Outlay.Infrastructure.Models.Responses;

namespace Outlay.Infrastructure.Mapper;

public class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<BrandFetchInfo, TransactionResponse>().ConvertUsing<TransactionConverter>();
        CreateMap<Transaction, TransactionByDescriptionResponse>()
            .ForMember(x => x.Amount, opt => opt.MapFrom(x => x.Amount / 100))
            .ForMember(x => x.Date, opt => opt.MapFrom(x => DateTimeOffset.FromUnixTimeSeconds(x.Time)
                .LocalDateTime.ToString(CultureInfo.InvariantCulture)))
            .ForMember(x => x.Name, opt => opt.MapFrom(x => x.Description));
        
        
    }
}