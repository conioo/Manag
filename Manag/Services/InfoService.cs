using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;

namespace Manag.Services
{
    public class InfoService : Info.InfoBase
    {
        public override Task<HealthResponse> CheckHealth(Empty request, ServerCallContext context)
        {
            var response = new HealthResponse() { Status = true };

            return Task.FromResult(response);
        }
    }
}