using Outlay.Extensions;
using Outlay.Infrastructure.Interfaces;
using Outlay.Infrastructure.Services;
using Outlay.Models;
using Outlay.Settings;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().Services
    .AddEndpointsApiExplorer()
    .AddSwaggerGen()
    .AddHttpClient(MonobankConstants.Client,
        httpClient =>
        {
            var settings = builder.Configuration.GetSection(SectionConstants.Monobank)
                .Get<MonobankSettings>();
            httpClient.BaseAddress = new Uri(settings!.BaseUrl);
            httpClient.DefaultRequestHeaders.Add(MonobankConstants.TokenHeader, settings.PersonalToken);
        }).Services
    .AddInMemoryDbContext()
    .AddRedis(builder.Configuration)
    .AddAutoMapper();

builder.Services.Configure<MonobankSettings>(x => builder.Configuration.GetSection(SectionConstants.Monobank).Bind(x));
builder.Services.Configure<BrandFetchSettings>(x => builder.Configuration.GetSection(BrandFetchConstants.Token).Bind(x));
builder.Services.AddScoped<IBrandFetchService, BrandFetchService>();
builder.Services.AddScoped<ICardService, CardService>();

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