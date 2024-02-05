using Grpc.Net.Client;
using System.IO.Pipes;
using System.Security.Principal;

namespace Server.Helpers
{
    internal static class GrpcHelper
    {
        internal static GrpcChannel CreateChannel()
        {
            //var connectionFactory = new NamedPipesConnectionFactory("square2");
            //var socketsHttpHandler = new SocketsHttpHandler
            //{
            //    ConnectCallback = connectionFactory.ConnectAsync
            //};

            //return GrpcChannel.ForAddress("http://localhost", new GrpcChannelOptions
            //{
            //    HttpHandler = socketsHttpHandler
            //});
            var channelOptions = new GrpcChannelOptions
            {
                MaxSendMessageSize = null, // 16 MB
                MaxReceiveMessageSize = null // 16 MB
            };

            return GrpcChannel.ForAddress($"http://localhost:6580", channelOptions);
        }
    }
    
    public class NamedPipesConnectionFactory
    {
        private readonly string pipeName;

        public NamedPipesConnectionFactory(string pipeName)
        {
            this.pipeName = pipeName;
        }

        public async ValueTask<Stream> ConnectAsync(SocketsHttpConnectionContext _,
            CancellationToken cancellationToken = default)
        {
            var clientStream = new NamedPipeClientStream(
                serverName: ".",
                pipeName: this.pipeName,
                direction: PipeDirection.InOut,
                options: PipeOptions.WriteThrough | PipeOptions.Asynchronous,
                impersonationLevel: TokenImpersonationLevel.Anonymous);

            try
            {
                await clientStream.ConnectAsync(cancellationToken).ConfigureAwait(false);
                return clientStream;
            }
            catch
            {
                clientStream.Dispose();
                throw;
            }
        }
    }
}
