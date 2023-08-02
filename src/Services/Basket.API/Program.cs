using Common.Logging;
using Serilog;
namespace Basket.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Host.UseSerilog(SerilogLogger.Configure);

            Log.Information("Starting Basket.API");
            try
            {
                {
                    // Add services to the container.
                    builder.Services.AddControllers();
                    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
                    builder.Services.AddEndpointsApiExplorer();
                    builder.Services.AddSwaggerGen();
                }

                var app = builder.Build();
                {
                    // Configure the HTTP request pipeline.
                    if (app.Environment.IsDevelopment())
                    {
                        app.UseSwagger();
                        app.UseSwaggerUI();
                    }

                    app.UseHttpsRedirection();

                    app.UseAuthorization();


                    app.MapControllers();
                }

                app.Run();
            }
            finally
            {
                Log.Fatal("Stopping Basket.API");
                Log.CloseAndFlush();
            }
        }
    }
}