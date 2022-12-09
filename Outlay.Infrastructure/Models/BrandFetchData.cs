namespace Outlay.Infrastructure.Models;

public class BrandFetchData
{
    public bool Claimed { get; set; }
    public string Name { get; set; }
    public string Domain { get; set; }
    public string Icon { get; set; }
}

public class BrandFetchInfo
{
    public string Name { get; set; }
    public int Amount { get; set; }
    public int Mcc { get; set; }
}
