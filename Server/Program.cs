using GrpcService.Services;
using Microsoft.AspNetCore.Builder;
using System.Reflection.PortableExecutable;

namespace Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            //var builder = Host.CreateApplicationBuilder(args);
            //builder.Services.AddHostedService<Worker>();
            //builder.Services.AddGrpc();

            //var host = builder.Build();

            //host.Run();

            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddGrpc();
            //builder.Host.UseWindowsService();

            var app = builder.Build();

            app.MapGrpcService<GreeterService>();
            app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");

            app.Run();

            //var options = new WebApplicationOptions
            //{
            //    Args = args,
            //    ContentRootPath = AppContext.BaseDirectory
            //};

            //var builder = WebApplication.CreateBuilder(options);

            //builder.Services.AddGrpc();
            //builder.Host.UseWindowsService();
            //var app = builder.Build();
            //app.MapGrpcService<eTutorServiceMain>();
            //app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            //app.Run();
        }

    }
}