using Common.Configuration;
using Serilog;
using Serilog.Settings.Configuration;
using Server.Services;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Host.UseWindowsService();
            builder.Host.UseSerilog((hostingContext, loggerConfiguration) =>
            {
                var assembly = typeof(ConsoleLoggerConfigurationExtensions).Assembly;
                var options = new ConfigurationReaderOptions(assembly);

#if DEBUG
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration);
#else
                loggerConfiguration.ReadFrom.Configuration(hostingContext.Configuration, options);
#endif
            });

            builder.Configuration.AddJsonFile("C:\\Users\\posce\\Documents\\Manag\\Common\\Configuration\\common.json");

            builder.Services.AddOptions<ApplicationOptions>().Bind(builder.Configuration.GetSection("AppSettings"));

            // Add services to the container.
            builder.Services.AddGrpc();
            builder.WebHost.UseUrls("http://localhost:6570");
            //options.Interceptors.Add<ErrorHandlingInterceptor>();
            var app = builder.Build();
            
            // Configure the HTTP request pipeline.
            //app.UseHttpsRedirection();
            app.MapGrpcService<WindowsSettingsService>();
            app.MapGrpcService<FileManagerService>();
            app.MapGrpcService<InfoService>();

            app.UseSerilogRequestLogging();

            //app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            app.Run();
        }
    }
}