namespace Outlay.Interfaces;

public interface IBrandFetchService
{
    Task<string> GetCompanyLogo(string companyName);
}