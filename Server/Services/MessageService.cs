using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Helpers;

namespace Server.Services
{
    public class MessageService : Message.MessageBase
    {
        public override async Task<Empty> ShowError(ErrorRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Message.MessageClient(channel);

            return await client.ShowErrorAsync(request);
        }

        public override async Task<Empty> ErrorInterval(ErrorIntervalMessage request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Message.MessageClient(channel);

            return await client.ErrorIntervalAsync(request);
        }

        public override async Task<MessageResponse> ShowMessage(MessageRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Message.MessageClient(channel);

            return await client.ShowMessageAsync(request);
        }
    }
}
