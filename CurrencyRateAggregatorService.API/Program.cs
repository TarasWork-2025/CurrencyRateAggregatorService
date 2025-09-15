using CurrencyRateAggregatorService.API.DTO;
using CurrencyRateAggregatorService.API.Middleware;
using CurrencyRateAggregatorService.Application.Contracts;
using CurrencyRateAggregatorService.Application.Services;
using CurrencyRateAggregatorService.Infrastructure.AppDbContext;
using CurrencyRateAggregatorService.Infrastructure.Caching;
using CurrencyRateAggregatorService.Infrastructure.RateProviders;
using CurrencyRateAggregatorService.Infrastructure.Repositories;
using CurrencyRateAggregatorService.Infrastructure.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Serilog;
using Microsoft.OpenApi.Any;

namespace CurrencyRateAggregatorService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddDbContext<CurrencyRateDbContext>(options =>
            {
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
            });

            builder.Services.AddScoped<IRateRepository, RateRepository>();
            builder.Services.AddScoped<IRateService, RateService>();

            // Added backgroud service to seed data to database.
            // Runs once with app start. Check console for details.
            // This is for DEMO PURPOSES ONLY.
            builder.Services.AddHostedService<RateSeeder>();

            Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", Serilog.Events.LogEventLevel.Warning)   
            .MinimumLevel.Override("System", Serilog.Events.LogEventLevel.Warning)      
            .MinimumLevel.Information()                                               
            .WriteTo.Console()
            .WriteTo.File("Logs/app.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();
  

            builder.Host.UseSerilog();

            builder.Services.AddHttpClient<NbuProvider>(client =>
            {
                var baseUrl = builder.Configuration["Nbu:BaseUrl"];
                client.BaseAddress = new Uri(baseUrl!);
            });

            builder.Services.AddMemoryCache();

            builder.Services.AddScoped<IRateProvider>(sp =>
            {
                var inner = sp.GetRequiredService<NbuProvider>();
                var cache = sp.GetRequiredService<IMemoryCache>();
                return new CachedRateProvider(inner, cache);
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            // Apply EF Core migrations automatically at startup
            // This is for DEMO PURPOSES ONLY, so the app 
            // works out-of-the-box without running `dotnet ef` commands.
            using (var scope = app.Services.CreateScope())
            {
                var db = scope.ServiceProvider.GetRequiredService<CurrencyRateDbContext>();
                db.Database.Migrate();
            }

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseExceptionHandling();

            app.UseHttpsRedirection();

            app.MapGet("/api/rates/{date}", async (DateOnly date, IRateService service, CancellationToken ct) =>
            {
                var result = await service.GetRateByDateAsync(date, ct);

                if (!result.IsValid)
                    return Results.BadRequest(new { Error = result.ErrorMessage });

                if (result.Value is null)
                    return Results.NotFound();

                return Results.Ok(new RateResponseDto
                {
                    Date = result.Value.Date,
                    BaseCurrency = result.Value.BaseCurrency,
                    QuoteCurrency = result.Value.QuoteCurrency,
                    Rate = result.Value.RatePer1
                });
            })
           .Produces<RateResponseDto>(StatusCodes.Status200OK)
           .Produces(StatusCodes.Status400BadRequest)
           .Produces(StatusCodes.Status404NotFound)
           .WithOpenApi(operation =>
           {
               operation.Parameters[0].Example = new OpenApiString("2025-09-16");
               return operation;
           });

            app.MapGet("/api/rates/average", async (DateOnly startDate, DateOnly endDate, IRateService service, CancellationToken ct) =>
            {
                var result = await service.GetAverageRateAsync(startDate, endDate, ct);

                if (!result.IsValid)
                    return Results.BadRequest(new { Error = result.ErrorMessage });

                if (!result.Value.HasValue)
                    return Results.NotFound();

                return Results.Ok(new AverageRateResponseDto
                {
                    StartDate = startDate,
                    EndDate = endDate,
                    Average = result.Value.Value
                });
            })
            .Produces<AverageRateResponseDto>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status400BadRequest)
            .Produces(StatusCodes.Status404NotFound)
            .WithOpenApi(operation =>
             {
                 operation.Parameters[0].Example = new OpenApiString("2025-09-01");
                 operation.Parameters[1].Example = new OpenApiString("2025-08-12");
                 return operation;
             });

            app.Run();
        }
    }
}
