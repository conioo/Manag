using CliWrap;
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

            if (args is { Length: 1 })
            {
                try
                {
                    string executablePath =
                        Path.Combine(AppContext.BaseDirectory, "Manag.exe");

                    if (args[0] is "/Install")
                    {
                    }
                    else if (args[0] is "/Uninstall")
                    {
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return;
            }

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
#if DEBUG
            var relativePath = @"./../Common/Configuration/common.json";
            var absolutePath = Path.GetFullPath(relativePath);

            builder.Configuration.AddJsonFile(absolutePath);
#else
            var absolutePath = Path.Combine(AppContext.BaseDirectory, "..", "common.json");
            builder.Configuration.AddJsonFile(absolutePath);
#endif

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