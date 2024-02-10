using Common.Configuration;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Server.Helpers;
using static Google.Protobuf.WindowsSettings;

namespace Server.Services
{
    public class WindowsSettingsService : WindowsSettings.WindowsSettingsBase
    {
        public WindowsSettingsService(IOptions<ApplicationOptions> options)
        {
            _appOptions = options.Value;
        }

        private readonly ApplicationOptions _appOptions;

        public override Task<Empty> ChangeWallpaper(WallpaperRequest request, ServerCallContext context)
        {
            try
            {
                var channel = GrpcHelper.CreateChannel();

                var client = new WindowsSettingsClient(channel);

                client.ChangeWallpaper(request);
            }
            catch(RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ChangeMouseSize(MouseRequest request, ServerCallContext context)
        {
            return Task.FromResult(new Empty());
        }
    }
}