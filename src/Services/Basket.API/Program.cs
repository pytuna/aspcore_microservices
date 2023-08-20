using Basket.API.Extensions;
using Common.Logging;
using Serilog;
namespace Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            var environment = builder.Environment;
            var applicationName = environment.ApplicationName;
            var environmentName = environment.EnvironmentName ?? "Development";
            Log.Information($"Starting ({applicationName})-({environmentName})...");

            try
            {
                builder.Host.UseSerilog(SerilogLogger.Configure);
                builder.Host.AddAppConfigurations();

                {
                    // Add services to the container.
                    builder.Services.AddControllers();
                    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                    builder.Services.Configure<RouteOptions>(options
                        => options.LowercaseUrls = true);

                    builder.Services.ConfigureServices();
                    builder.Services.ConfigureRedis(builder.Configuration);
                    builder.Services.AddConfigurationSettings(builder.Configuration);
                    builder.Services.ConfigureMassTransit();
                    builder.Services.AddAutoMapper(cfgs => cfgs.AddProfile(new MappingProfile()));
                }

                var app = builder.Build();
                {
                    // Configure the HTTP request pipeline.
                    if (app.Environment.IsDevelopment())
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();
                    }

                    //app.UseHttpsRedirection();

                    app.UseAuthorization();

                    app.MapControllers();
                }

                app.Run();
            }
            catch (Exception ex)
            {
                string type = ex.GetType().Name;
                if (type.Equals("StopTheHostException", StringComparison.Ordinal)) throw;

                Log.Fatal(ex, "Basket.API terminated unexpectedly");
            }
            finally
            {
                Log.Fatal("Stopping Basket.API");
                Log.CloseAndFlush();
            }
        }
    }
}