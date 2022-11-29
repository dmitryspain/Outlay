using Microsoft.Extensions.Options;
using Outlay.Extensions;
using Outlay.Interfaces;
using Outlay.Models;
using Outlay.Services;
using Outlay.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.Configure<MonobankSettings>(x => builder.Configuration.GetSection(MonobankConstants.Section).Bind(x));
builder.Services.Configure<BrandFetchSettings>(x => builder.Configuration.GetSection(BrandFetchConstants.Token).Bind(x));
builder.Services.AddHttpClient(MonobankConstants.Client, 
    httpClient =>
    {
        var settings = builder.Services
            .BuildServiceProvider()
            .GetRequiredService<IOptions<MonobankSettings>>();
        
        httpClient.BaseAddress = new Uri(settings.Value.BaseUrl);
        httpClient.DefaultRequestHeaders.Add(MonobankConstants.TokenHeader, settings.Value.PersonalToken);
    });
builder.Services.AddInMemoryDbContext();
builder.Services.AddScoped<IBrandFetchService, BrandFetchService>();

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors(x => x
    .AllowAnyOrigin()
    .AllowAnyMethod()
    .AllowAnyHeader());

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();