using AutoMapper;
using Outlay.Infrastructure.InMemoryDb;
using Outlay.Infrastructure.Interfaces;
using Outlay.Infrastructure.Models;
using Outlay.Infrastructure.Models.Responses;

namespace Outlay.Infrastructure.Converters;

public class TransactionConverter : ITypeConverter<BrandFetchInfo, TransactionResponse>
{
    private readonly IBrandFetchService _brandFetchService;
    private readonly OutlayInMemoryContext _inMemoryContext;

    public TransactionConverter(IBrandFetchService brandFetchService, OutlayInMemoryContext inMemoryContext)
    {
        _brandFetchService = brandFetchService;
        _inMemoryContext = inMemoryContext;
    }
    public TransactionResponse Convert(BrandFetchInfo source, TransactionResponse destination, ResolutionContext context)
    {
        return new TransactionResponse
        {
            Name = source.Name,
            Amount = source.Amount,
            Icon = _brandFetchService.GetCompanyLogo(source.Name).Result,
            Category = _inMemoryContext.MccInfos.FirstOrDefault(x => x.Mcc == source.Mcc)!.ShortDescription
        };
    }
}