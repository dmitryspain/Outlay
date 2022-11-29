using Microsoft.Extensions.Options;
using Outlay.Interfaces;
using Outlay.Models;
using Outlay.Settings;

namespace Outlay.Services;

public class BrandFetchService : IBrandFetchService
{
    private readonly string _token;
    private readonly HttpClient _client;

    public BrandFetchService(IOptions<BrandFetchSettings> options, IHttpClientFactory factory)
    {
        _token = options.Value.Token;
        _client = factory.CreateClient();
    }
    public async Task<string> GetCompanyLogo(string companyName)
    {
        // todo rewrite
        var searchUrl = $"https://api.brandfetch.io/v2/search/{companyName}";
        var message = await _client.GetAsync(searchUrl);
        var result = (await message.Content.ReadFromJsonAsync<IEnumerable<BrandFetchData>>())!.ToList();
        if (result is null || !result.Any())
            return "";
        return result!.FirstOrDefault().Icon;
    }
}