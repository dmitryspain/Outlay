using Microsoft.AspNetCore.Mvc;
using Outlay.Db;
using Outlay.Interfaces;
using Outlay.Models;

namespace Outlay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutlayController : ControllerBase
{
    private readonly OutlayInMemoryContext _context;
    private readonly IBrandFetchService _brandFetchService;
    private const string CardId1 = "XzNHt-dLmlqphOajbo0saA";
    private const string CardId2 = "M5qwTR5-FL_F0ZrJYzngKw";
    private readonly HttpClient _httpClient;

    public OutlayController(IHttpClientFactory httpClient, OutlayInMemoryContext context, IBrandFetchService brandFetchService)
    {
        _context = context;
        _brandFetchService = brandFetchService;
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
    public async Task<ActionResult<IEnumerable<object>>> GetCardHistoryGrouped()
    {
        var unixSeconds = DateTimeOffset.Now.AddDays(-30).ToUnixTimeSeconds();
        var result = await _httpClient.GetAsync($"/personal/statement/{CardId1}/{unixSeconds}");
        var cardHistories = await result.Content.ReadFromJsonAsync<IEnumerable<CardHistory>>();
        var res = cardHistories!.GroupBy(x => x.Description)
            .Select(x => new
            {
                Name = x.Key, Amount = x.Sum(s => s.Amount / 100),
                Category = _context.MccInfos
                    .FirstOrDefault(m => m.Mcc == x.Select(s => s.Mcc)
                        .FirstOrDefault())!.ShortDescription,
                Icon = _brandFetchService.GetCompanyLogo(x.Key)
            })
            .OrderBy(x => x.Amount);

        return Ok(res);
    }
}