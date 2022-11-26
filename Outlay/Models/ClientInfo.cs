namespace Outlay.Models;

public class ClientInfo
{
    public string ClientId { get; set; }
    public string Name { get; set; }
    public string WebHookUrl { get; set; }
    public string Permissions { get; set; }
    public IEnumerable<ClientAccount> Accounts { get; set; }
}
