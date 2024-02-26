using CliWrap;
using Common.Configuration;
using Serilog;
using Serilog.Settings.Configuration;
using Server.Helpers;
using Server.Interceptors;
using Server.Services;

namespace Server
{
    public class Program
    {
        const int PORT = 6570;
        const string SERVICENAME = "abecadlo";

        public static void Main(string[] args)
        {
            if (args is { Length: 1 })
            {
                try
                {
                    string executablePath =
                        Path.Combine(AppContext.BaseDirectory, "Server.exe");

                    if (args[0] is "/Install")
                    {
                        FirewallHelper.AddFirewallRule("aabeb", executablePath, PORT);

                        Cli.Wrap("sc")
                            .WithArguments(new[] { "stop", SERVICENAME })
                            .ExecuteAsync().Task.Wait();

                        Cli.Wrap("sc")
                            .WithArguments(new[] { "delete", SERVICENAME })
                            .ExecuteAsync().Task.Wait();

                        Cli.Wrap("sc")
                            .WithArguments(new[] { "create", SERVICENAME, $"binPath={executablePath}", "start=auto", })
                            .ExecuteAsync().Task.Wait();

                        Cli.Wrap("sc")
                            .WithArguments(new[] { "failure", SERVICENAME, "reset=0", "actions=restart/1000/run" })
                            .ExecuteAsync().Task.Wait();

                        Cli.Wrap("sc")
                            .WithArguments(new[] { "start", SERVICENAME })
                            .ExecuteAsync().Task.Wait();

                    }
                    else if (args[0] is "/Uninstall")
                    {
                        FirewallHelper.RemoveFirewallRule("aabeb");

                        Cli.Wrap("sc")
                            .WithArguments(new[] { "stop", SERVICENAME })
                            .ExecuteAsync().Task.Wait();

                        Cli.Wrap("sc")
                            .WithArguments(new[] { "delete", SERVICENAME })
                            .ExecuteAsync().Task.Wait();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }

                return;
            }

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

#if DEBUG
            var relativePath = @"./../Common/Configuration/common.json";
            var absolutePath = Path.GetFullPath(relativePath);

            builder.Configuration.AddJsonFile(absolutePath);
#else
            var absolutePath = Path.Combine(AppContext.BaseDirectory, "..", "common.json");
            builder.Configuration.AddJsonFile(absolutePath);
#endif

            builder.Services.AddOptions<ApplicationOptions>().Bind(builder.Configuration.GetSection("AppSettings"));

            // Add services to the container.
            builder.Services.AddGrpc(options =>
            {
                options.MaxReceiveMessageSize = null;
                options.MaxSendMessageSize = null;
                options.Interceptors.Add<DelayInterceptor>();
            });

            builder.WebHost.UseUrls($"http://0.0.0.0:{PORT}");


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            //app.UseHttpsRedirection();
            app.MapGrpcService<WindowsSettingsService>();
            app.MapGrpcService<FileManagerService>();
            app.MapGrpcService<InfoService>();
            app.MapGrpcService<ProcessService>();
            app.MapGrpcService<AudioService>();
            app.MapGrpcService<MessageService>();
            app.MapGrpcService<CameraService>();

            app.UseSerilogRequestLogging();

            //app.MapGet("/", () => "Communication with gRPC endpoints must be made through a gRPC client. To learn how to create a client, visit: https://go.microsoft.com/fwlink/?linkid=2086909");
            app.Run();
        }
    }
}