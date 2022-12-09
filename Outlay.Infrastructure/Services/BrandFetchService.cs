using System.Net.Http.Json;
using Outlay.Infrastructure.Interfaces;
using Outlay.Infrastructure.Models;

namespace Outlay.Infrastructure.Services;

public class BrandFetchService : IBrandFetchService
{
    // private readonly HttpClient _client;
    //
    // public BrandFetchService(IHttpClientFactory factory)
    // {
    //     _client = factory.CreateClient();
    // }
    
    public async Task<string> GetCompanyLogo(string companyName)
    {
        var searchUrl = $"https://api.brandfetch.io/v2/search/{companyName}";
        using var client = new HttpClient();
        var message = await client.GetAsync(searchUrl);
        var brandFetchIcons = (await message.Content.ReadFromJsonAsync<IEnumerable<BrandFetchData>>())!.ToList();
        if (brandFetchIcons is null || !brandFetchIcons.Any())
            return string.Empty;
        
        // From the list of icons get first (best match)
        return brandFetchIcons.FirstOrDefault()!.Icon;
    }
}