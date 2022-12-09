using Outlay.Infrastructure.Models;
using Outlay.Infrastructure.Models.Requests;
using Outlay.Infrastructure.Models.Responses;

namespace Outlay.Infrastructure.Interfaces;

public interface ICardService
{
    Task<IEnumerable<TransactionResponse>> GetCardHistoryGrouped(CardHistoryRequest request, CancellationToken cancellationToken);

    Task<IEnumerable<TransactionByDescriptionResponse>> GetCardHistoryByDescription(CardHistoryByDescriptionRequest request,
        CancellationToken cancellationToken);

    Task<IEnumerable<Transaction>> GetCardHistory(string cardId, CancellationToken cancellationToken);
    Task<ClientInfoResponse> GetClientInfo(CancellationToken cancellationToken);
}