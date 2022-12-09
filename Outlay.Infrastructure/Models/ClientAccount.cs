namespace Outlay.Infrastructure.Models;

public class ClientAccount
{
    public string Id { get; set; }
    public string SendId { get; set; }
    public int Balance { get; set; }
    public int CreditLimit { get; set; }
    public string Type { get; set; }
    public int CurrencyCode { get; set; }
    public string CurrencyType { get; set; }
    public string Iban { get; set; }
    public IEnumerable<string> MaskedPan { get; set; }
}