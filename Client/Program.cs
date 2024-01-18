using Google.Protobuf;
using Grpc.Net.Client;
//using Server;

namespace Client
{
    internal class Program
    {
        static async Task Main(string[] args)
        {
            //using var channel = GrpcChannel.ForAddress("http://localhost:5000");
            using var channel = GrpcChannel.ForAddress("https://localhost:7018");

            var client = new Greeter.GreeterClient(channel);
            var client2 = new Command.CommandClient(channel);



            //var reply = await client.SayHelloAsync(
            //                  new HelloRequest { Name = "GreeterClient" });
            //Console.WriteLine("Greeting: " + reply.Message);
            //Console.WriteLine("Press any key to exit...");

            //var reply2 = await client2.SendAsync(
            //                  new HelloRequest { Name = "GreeterClient" });
            //Console.WriteLine(reply2.Message);
            //Console.ReadKey();
        }
    }
}
