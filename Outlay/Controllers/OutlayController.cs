using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using Outlay.Db;
using Outlay.Infrastructure.Extensions;
using Outlay.Interfaces;
using Outlay.Models;

namespace Outlay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutlayController : ControllerBase
{
    private readonly OutlayInMemoryContext _context;
    private readonly IBrandFetchService _brandFetchService;
    private readonly IDistributedCache _cache;
    private const string CardId1 = "XzNHt-dLmlqphOajbo0saA";
    private const string CardId2 = "M5qwTR5-FL_F0ZrJYzngKw";
    private readonly HttpClient _httpClient;

    public OutlayController(IHttpClientFactory httpClient, OutlayInMemoryContext context,
        IBrandFetchService brandFetchService, IDistributedCache cache)
    {
        _context = context;
        _brandFetchService = brandFetchService;
        _cache = cache;
        _httpClient = httpClient.CreateClient(MonobankConstants.Client);
    }

    [HttpGet("client-info")]
    public async Task<ActionResult<ClientInfo>> GetClientInfo()
    {
        var result = await _httpClient.GetAsync("/personal/client-info");
        var clientInfo = await result.Content.ReadFromJsonAsync<ClientInfo>();
        return Ok(clientInfo);
    }

    [HttpGet("card-history")]
    public async Task<ActionResult<IEnumerable<CardHistory>>> GetCardHistory()
    {
        var unixSeconds = DateTimeOffset.Now.AddDays(-1).ToUnixTimeSeconds();
        var result = await _httpClient.GetAsync($"/personal/statement/{CardId1}/{unixSeconds}");
        var cardHistories = await result.Content.ReadFromJsonAsync<IEnumerable<CardHistory>>();
        return Ok(cardHistories);
    }

    [HttpGet("card-history-grouped")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetCardHistoryGrouped(
        CancellationToken cancellationToken)
    {
        const string cacheKey = "card-history";
        var cached = await _cache.GetRecordAsync<List<TransactionResponse>>(cacheKey);
        if (cached != null)
            return Ok(cached);

        var unixSeconds = DateTimeOffset.Now.AddDays(-10).ToUnixTimeSeconds();
        var httpResponse = await _httpClient.GetAsync($"/personal/statement/{CardId1}/{unixSeconds}",
            cancellationToken);
        var cardHistories = await httpResponse.Content.ReadFromJsonAsync
            <IEnumerable<CardHistory>>(cancellationToken: cancellationToken);
        var infos = cardHistories!.GroupBy(x => x.Description)
            .Select(x => new BrandFetchInfo
            {
                Name = x.Key, Amount = x.Sum(s => s.Amount / 100),
                Mcc = x.FirstOrDefault()!.Mcc
            })
            .OrderBy(x => x.Amount);

        var transactions = await GetResultModelAsync(infos);
        await _cache.SetRecordAsync(cacheKey, absoluteExpireTime: TimeSpan.FromHours(1), data: transactions);

        // Mock
        // using var r = new StreamReader("grouped.json");
        // var json = await r.ReadToEndAsync(cancellationToken);
        // var mockedItems = JsonConvert.DeserializeObject<IEnumerable<TransactionResponse>>(json);

        return Ok(transactions);
    }

    [HttpGet("card-history-by-description")]
    public async Task<ActionResult<IEnumerable<TransactionByCategoryResponse>>> GetCardHistoryByDescription(
        [FromQuery] string description,
        CancellationToken cancellationToken)
    {
        const string cacheKey = "card-history-by-description";
        var cached = await _cache.GetRecordAsync<List<TransactionByCategoryResponse>>(cacheKey);
        if (cached != null)
            return Ok(cached);
        var unixSeconds = DateTimeOffset.Now.AddDays(-10).ToUnixTimeSeconds();
        var allTransactions =
            await _httpClient.GetAsync($"/personal/statement/{CardId1}/{unixSeconds}", cancellationToken);
        var filtered = (await allTransactions.Content.ReadFromJsonAsync<IEnumerable<CardHistory>>(
            cancellationToken: cancellationToken))!.Where(x => x.Description == description);
        // // Mock
        // using var r = new StreamReader("transactions.json");
        // var json = await r.ReadToEndAsync(cancellationToken);
        // var mockedItems = JsonConvert.DeserializeObject<IEnumerable<CardHistory>>(json);
        var res = filtered!.Where(x => x.Description == description)
            .Select(x => new TransactionByCategoryResponse
            {
                Test = DateTimeOffset.FromUnixTimeSeconds(x.Time),
                Name = x.Description,
                Date = $"{DateTimeOffset.FromUnixTimeSeconds(x.Time).LocalDateTime}",
                Amount = x.Amount / 100
            });

        await _cache.SetRecordAsync(cacheKey, absoluteExpireTime: TimeSpan.FromHours(1), data: res);
        return Ok(res);
    }

    private async Task<IEnumerable<TransactionResponse>> GetResultModelAsync(
        IEnumerable<BrandFetchInfo> brandFetchInfos)
    {
        var transactionResponses = new List<TransactionResponse>();
        foreach (var brandFetchInfo in brandFetchInfos)
        {
            var newTransaction = new TransactionResponse()
            {
                Amount = brandFetchInfo.Amount,
                Name = brandFetchInfo.Name,
                Category = (await _context.MccInfos.FirstOrDefaultAsync(x => x.Mcc == brandFetchInfo.Mcc))!
                    .ShortDescription,
                Icon = await _brandFetchService.GetCompanyLogo(brandFetchInfo.Name)
            };
            transactionResponses.Add(newTransaction);
        }

        return transactionResponses;
    }
}