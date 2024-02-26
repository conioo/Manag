using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Server.Helpers;

namespace Server.Services
{
    public class CameraService : Camera.CameraBase
    {
        public async override Task<PhotoResponse> Photo(PhotoRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Camera.CameraClient(channel);

            return await client.PhotoAsync(request);
        }

        public async override Task<VideoResponse> Video(VideoRequest request, ServerCallContext context)
        {
            var channel = GrpcHelper.CreateChannel();

            var client = new Camera.CameraClient(channel);

            return await client.VideoAsync(request);
        }
    }
}