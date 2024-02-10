using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Helpers;

namespace Server.Services
{
    public class AudioService : Audio.AudioBase
    {
        public async override Task<Empty> ChangeVolume(VolumeRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.ChangeVolumeAsync(request);
        }

        public async override Task<Empty> Mute(MuteRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.MuteAsync(request);
        }

        public async override Task<Empty> Play(PlayRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.PlayAsync(request);
        }

        public async override Task<Empty> SystemPlay(SystemPlayRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.SystemPlayAsync(request);
        }

        public async override Task<Empty> Record(RecordRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.RecordAsync(request);
        }
    }
}
