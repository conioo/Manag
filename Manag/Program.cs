using Common.Configuration;
using Common.Interfaces;
using Common.Services;
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
            builder.Services.AddSingleton<IFileService, FileService>();
            builder.Services.AddGrpc(options =>
            {
                options.MaxReceiveMessageSize = null;
                options.MaxSendMessageSize = null;
            });

            builder.WebHost.UseUrls("http://localhost:6580");

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            app.MapGrpcService<InfoService>();
            app.MapGrpcService<WindowsSettingsService>();
            app.MapGrpcService<ProcessService>();
            app.MapGrpcService<AudioService>();
            app.MapGrpcService<MessageService>();
            app.MapGrpcService<CameraService>();

            app.Run();
        }
    }
}