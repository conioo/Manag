using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Helpers;

namespace Server.Services
{
    public class ProcessService : Process.ProcessBase
    {
        public override async Task<ProcessesResponse> GetProcesses(Empty request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Process.ProcessClient(channel);

            return await client.GetProcessesAsync(request);
        }

        public override async Task<Empty> NewIntervalProcess(IntervalRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);

            var channel = GrpcHelper.CreateChannel();

            var client = new Process.ProcessClient(channel);

            return await client.NewIntervalProcessAsync(request);
        }

        public override async Task<Empty> NewProcess(ProcessRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);

            var channel = GrpcHelper.CreateChannel();

            var client = new Process.ProcessClient(channel);

            return await client.NewProcessAsync(request);
        }

        public override async Task<ShutdownResponse> ShutdownProcess(ShutdownRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);

            var channel = GrpcHelper.CreateChannel();

            var client = new Process.ProcessClient(channel);

            return await client.ShutdownProcessAsync(request);
        }

        public override async Task<Empty> ShutdownWindows(ShutdownWindowsRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);

            var channel = GrpcHelper.CreateChannel();

            var client = new Process.ProcessClient(channel);

            return await client.ShutdownWindowsAsync(request);
        }
    }
}