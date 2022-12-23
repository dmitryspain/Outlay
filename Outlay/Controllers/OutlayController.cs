using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Outlay.Infrastructure.Extensions;
using Outlay.Infrastructure.Interfaces;
using Outlay.Infrastructure.Models;
using Outlay.Infrastructure.Models.Requests;
using Outlay.Infrastructure.Models.Responses;

namespace Outlay.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OutlayController : ControllerBase
{
    private readonly ICardService _cardService;
    private readonly IDistributedCache _cache;

    public OutlayController(ICardService cardService, IDistributedCache cache)
    {
        _cardService = cardService;
        _cache = cache;
    }

    [HttpGet("client-info")]
    public async Task<ActionResult<ClientInfo>> GetClientInfo(CancellationToken cancellationToken)
    {
        var clientInfo = await _cardService.GetClientInfo(cancellationToken);
        clientInfo.CardInfos = clientInfo.CardInfos.Where(x => x.Balance > 0); 
        return Ok(clientInfo);
    }

    [HttpGet("card-history")]
    public async Task<ActionResult<IEnumerable<Transaction>>> GetCardHistory(string cardId,
        CancellationToken cancellationToken)
    {
        var cardHistories = await _cardService.GetCardHistory(cardId, cancellationToken);
        return Ok(cardHistories);
    }

    [HttpGet("card-history-grouped")]
    public async Task<ActionResult<IEnumerable<TransactionResponse>>> GetCardHistoryGrouped(
        [FromQuery] CardHistoryRequest cardRequest,
        CancellationToken cancellationToken)
    {
        const string cacheKey = "card-history";
        var cached = await _cache.GetRecordAsync<List<TransactionResponse>>(cacheKey);
        if (cached is not null)
            return Ok(cached);

        var transactions = await _cardService.GetCardHistoryGrouped(cardRequest, cancellationToken);
        await _cache.SetRecordAsync(cacheKey, absoluteExpireTime: TimeSpan.FromHours(1), data: transactions);
        return Ok(transactions);
    }

    [HttpGet("card-history-by-description")]
    public async Task<ActionResult<IEnumerable<TransactionByDescriptionResponse>>> GetCardHistoryByDescription(
        [FromQuery] CardHistoryByDescriptionRequest request,
        CancellationToken cancellationToken)
    {
        var transactions = await _cardService.GetCardHistoryByDescription(request, cancellationToken);
        return Ok(transactions);
    }
}