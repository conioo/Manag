using Common.Configuration;
using Manag.Services;
using Server.Services;

namespace Manag
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            //builder.WebHost.ConfigureKestrel(serverOptions =>
            //{
            //    serverOptions.ListenNamedPipe("square2", listenOptions =>
            //    {
            //        //NamedPipeServerStreamAcl.Create()
            //        listenOptions.Protocols = HttpProtocols.Http2;
            //        listenOptions.DisableAltSvcHeader = true;
            //    });
            //});

            // Add services to the container.
            builder.Configuration.AddJsonFile("C:\\Users\\posce\\Documents\\Manag\\Common\\Configuration\\common.json");
            builder.Services.AddOptions<ApplicationOptions>().Bind(builder.Configuration.GetSection("AppSettings"));

            builder.Services.AddGrpc();
            builder.WebHost.UseUrls("http://localhost:6580");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<InfoService>();
            app.MapGrpcService<WindowsSettingsService>();

            app.Run();
        }
    }
}