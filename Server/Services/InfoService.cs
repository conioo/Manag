using Common.Helpers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Helpers;
using System.Diagnostics;
using System.IO.Pipes;
using System.Security.Principal;
using Process = System.Diagnostics.Process;

namespace Server.Services
{
    public class InfoService : Info.InfoBase
    {
        private readonly ILogger<InfoService> _logger;

        public InfoService(ILogger<InfoService> logger)
        {
            _logger = logger;
        }
        public override Task<HealthResponse> CheckHealth(Empty request, ServerCallContext context)
        {
            var response = new HealthResponse() { Status = true };

            //var _channel = CreateChannel();
            //var cc = new InfoClient(_channel);
            //cc.CheckHealth(new Empty());
            //AudioHelper.changeVolume(0);
            return Task.FromResult(response);
        }

        public override Task<Empty> StartSession(Empty request, ServerCallContext context)
        {
            killManagProcess();
#if DEBUG
            Process process = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\posce\Documents\Manag\Manag\bin\Release\net8.0\publish\Manag.exe",
                    CreateNoWindow = false,
                    //RedirectStandardOutput = true,
                    //UseShellExecute = false
                }
            };
            process.Start();
#else
            ProcessHelper.StartProcessAsCurrentUser(@"C:\Users\posce\Documents\Manag\Manag\bin\Release\net8.0\publish\Manag.exe", visible: false);
#endif
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> EndSession(Empty request, ServerCallContext context)
        {
            killManagProcess();
            _logger.LogInformation("End session");

            return Task.FromResult(new Empty());
        }

        private void killManagProcess()
        {
            var processes = Process.GetProcessesByName("Manag");

            if (processes.Length > 0)
            {
                foreach (var process in processes)
                {
                    process.Kill();
                }
            }
        }
    }
}