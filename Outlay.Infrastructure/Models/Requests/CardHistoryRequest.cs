namespace Outlay.Infrastructure.Models.Requests;

public class CardHistoryRequest
{
   public string CardId { get; set; }
   public DateTime DateFrom { get; set; }
   public DateTime DateTo { get; set; }
}