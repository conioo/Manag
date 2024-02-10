using Common.Configuration;
using Common.Helpers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using System.Diagnostics;
using Process = System.Diagnostics.Process;

namespace Server.Services
{
    public class ProcessService : Google.Protobuf.Process.ProcessBase
    {
        public ProcessService(ILogger<ProcessService> logger, IOptions<ApplicationOptions> options)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        private readonly ApplicationOptions _appSettings;
        private readonly ILogger<ProcessService> _logger;

        public override Task<ProcessesResponse> GetProcesses(Empty request, ServerCallContext context)
        {
            var processes = Process.GetProcesses();
            var processesNames = new List<string>();

            foreach (var process in processes)
            {
                processesNames.Add(process.ProcessName);
            }

            var response = new ProcessesResponse() { ProcessNames = { processesNames } };

            return Task.FromResult(response);
        }

        public override Task<Empty> NewProcess(ProcessRequest request, ServerCallContext context)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = request.ProcessName,
                Arguments = request.Arguments,
                UseShellExecute = true
            };

            AudioHelper.ChangeVolume(request.Volume);

            for (int i = 0; i < request.Count; ++i)
            {
                Process process = new Process() { StartInfo = startInfo };
                process.Start();
            }

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> NewIntervalProcess(IntervalRequest request, ServerCallContext context)
        {
            Interval(request);

            return Task.FromResult(new Empty());
        }

        public override Task<ShutdownResponse> ShutdownProcess(ShutdownRequest request, ServerCallContext context)
        {
            var response = new ShutdownResponse() { CountShutdownProcesses = 0 };

            if (request.IsAll is not null && request.IsAll is true)
            {
                foreach (Process process in Process.GetProcesses())
                {
                    if(process.ProcessName == "Manag")
                    {
                        continue;
                    }

                    if (process.CloseMainWindow())
                    {
                        response.CountShutdownProcesses++;
                    }
                }
            }
            else
            {
                if(request.ProcessName == "Manag")
                {
                    return Task.FromResult(response);
                }

                var processes = Process.GetProcessesByName(request.ProcessName);

                if (processes.Length == 0)
                {
                    return Task.FromResult(response);
                }

                foreach (Process process in processes)
                {
                    process.CloseMainWindow();
                    response.CountShutdownProcesses++;
                }
            }

            return Task.FromResult(response);
        }

        public override Task<Empty> ShutdownWindows(ShutdownWindowsRequest request, ServerCallContext context)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = "shutdown",
                Arguments = "/s /t 0 /f",
                CreateNoWindow = true,
                UseShellExecute = false
            };

            Process process = new Process { StartInfo = startInfo };

            process.Start();

            return Task.FromResult(new Empty());
        }
        private async Task Interval(IntervalRequest request)
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = request.ProcessName,
                Arguments = request.Arguments,
                UseShellExecute = true
            };

            for (int i = 0; i < request.Count; ++i)
            {
                AudioHelper.ChangeVolume(request.Volume);

                for (int k = 0; k < request.CountPerInterval; ++k)
                {
                    Process process = new Process() { StartInfo = startInfo };
                    process.Start();
                }

                Thread.Sleep(request.IntervalInSeconds * 1000);
            }
        }
    }
}