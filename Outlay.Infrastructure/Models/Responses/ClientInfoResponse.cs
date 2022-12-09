namespace Outlay.Infrastructure.Models.Responses;

public class ClientInfoResponse
{
    public string Name { get; set; }
    public IEnumerable<CardInfoResponse> CardInfos { get; set; }
}