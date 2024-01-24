using Google.Protobuf;
using Grpc.Core;

namespace Server.Services
{
    public class CommandService: Command.CommandBase
    {
        public override Task<HelloReply> Send(HelloRequest request, ServerCallContext context)
        {
            Console.WriteLine("send");

            var er = new HelloReply();
            er.Message = "gowno";

            return Task.FromResult(er);
        }
    }
}
