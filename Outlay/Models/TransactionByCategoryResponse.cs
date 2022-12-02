namespace Outlay.Models;

public class TransactionByCategoryResponse
{
    public string Name { get; set; }
    public DateTimeOffset Time { get; set; }
    public int Amount { get; set; }
}