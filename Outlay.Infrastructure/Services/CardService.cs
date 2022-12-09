using System.Globalization;
using System.Net.Http.Json;
using AutoMapper;
using Outlay.Infrastructure.Interfaces;
using Outlay.Infrastructure.Models;
using Outlay.Infrastructure.Models.Requests;
using Outlay.Infrastructure.Models.Responses;

namespace Outlay.Infrastructure.Services;

public class CardService : ICardService
{
    private readonly HttpClient _httpClient;
    // private readonly OutlayInMemoryContext _context;
    // private readonly IBrandFetchService _brandFetchService;
    private readonly IMapper _mapper;
    
    
    public CardService(
        IHttpClientFactory httpClientFactory, IMapper mapper)
    {
        // _context = context;
        // _brandFetchService = brandFetchService;
        _mapper = mapper;
        _httpClient = httpClientFactory.CreateClient(MonobankConstants.Client);
    }
    
    public async Task<IEnumerable<TransactionResponse>> GetCardHistoryGrouped(CardHistoryRequest request,
        CancellationToken cancellationToken)
    {
        var unixSeconds = DateTimeOffset.Now.AddDays(-10).ToUnixTimeSeconds();
        var httpResponse = await _httpClient.GetAsync($"/personal/statement/{request.CardId}/{unixSeconds}",
            cancellationToken);
        var cardHistories = await httpResponse.Content.ReadFromJsonAsync
            <IEnumerable<Transaction>>(cancellationToken: cancellationToken);
        var infos = cardHistories!.GroupBy(x => x.Description)
            .Select(x => new BrandFetchInfo
            {
                Name = x.Key, 
                Amount = x.Sum(s => s.Amount / 100), // simple case, ignore fractional part 
                Mcc = x.FirstOrDefault()!.Mcc
            })
            .OrderBy(x => x.Amount)
            .AsEnumerable();

        var transactions = _mapper.Map<IEnumerable<TransactionResponse>>(infos);
        return transactions;
    }

    public async Task<IEnumerable<TransactionByDescriptionResponse>> GetCardHistoryByDescription(CardHistoryByDescriptionRequest request, CancellationToken cancellationToken)
    {
        var unixSeconds = DateTimeOffset.Now.AddDays(-10).ToUnixTimeSeconds();
        var allTransactions = await _httpClient.GetAsync($"/personal/statement/{request.CardId}/{unixSeconds}",
            cancellationToken);
        
        var filtered = (await allTransactions.Content.ReadFromJsonAsync<IEnumerable<Transaction>>(cancellationToken:
            cancellationToken))!.Where(x => x.Description == request.Description);

        var response = _mapper.Map<IEnumerable<TransactionByDescriptionResponse>>(filtered);
        return response;
    }

    public async Task<IEnumerable<Transaction>> GetCardHistory(string cardId, CancellationToken cancellationToken)
    {
        var unixSeconds = DateTimeOffset.Now.AddDays(-1).ToUnixTimeSeconds();
        var result = await _httpClient.GetAsync($"/personal/statement/{cardId}/{unixSeconds}", cancellationToken);
        var cardHistories = await result.Content.ReadFromJsonAsync<IEnumerable<Transaction>>(cancellationToken:
            cancellationToken);
        return cardHistories;
    }

    public async Task<ClientInfoResponse> GetClientInfo(CancellationToken cancellationToken)
    {
        var result = await _httpClient.GetAsync("/personal/client-info", cancellationToken);
        var clientInfo = await result.Content.ReadFromJsonAsync<ClientInfo>(cancellationToken: cancellationToken);
        var response = _mapper.Map<ClientInfoResponse>(clientInfo);
        return response!;
    }
}