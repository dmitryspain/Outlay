namespace Outlay.Infrastructure.Models.Responses;

public class CardInfoResponse
{
    public string Id { get; set; }
    public int Balance { get; set; }
    public int CurrencyCode { get; set; }
    public string Type { get; set; }
    public string MaskedCardNumber { get; set; }
}