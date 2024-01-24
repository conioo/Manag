using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Server.Configuration;
using Server.Interceptors;
using Server.Services;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseSerilog((hostingContext, loggerConfiguration) => loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration));

            builder.Services.AddOptions<ApplicationOptions>().Bind(builder.Configuration.GetSection("AppSettings"));

            builder.Host.UseWindowsService();
            // Add services to the container.
            builder.Services.AddGrpc().AddServiceOptions<GreeterService>(options =>
            {
                //options.Interceptors.Add<ErrorHandlingInterceptor>();
            }); ;


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //app.UseHttpsRedirection();
            //app.MapGrpcService<GreeterService>();
            //app.MapGrpcService<CommandService>();
            //app.MapGrpcService<FileManagerService>();
            app.MapGrpcService<WindowsSettingsService>();
            app.MapGrpcService<FileManagerService>();
            app.MapGrpcService<InfoService>();

            app.UseSerilogRequestLogging();

            //app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();
        }
    }
}