namespace Outlay.Infrastructure.Interfaces;

public interface IBrandFetchService
{
    Task<string> GetCompanyLogo(string companyName);
}