using System.Text.Json;
using Outlay.Models;

namespace Outlay.Db;

public static class MccInfoInitializer
{
    public static void AddMccs(OutlayInMemoryContext context)
    {
        using var httpClient = new HttpClient();
        const string mccCodesPath = "https://raw.githubusercontent.com/Oleksios/Merchant-Category-Codes/main/Without%20groups/mcc-uk.json";
        var records = httpClient.GetFromJsonAsync<IEnumerable<MccInfo>>(mccCodesPath).Result;
        // var objects = JsonSerializer.Deserialize<IEnumerable<MccInfo>>(records);
        context.AddRange(records!);
        context.SaveChangesAsync();
    }
}