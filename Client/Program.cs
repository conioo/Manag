using Client.Commands;
using Grpc.Core;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;

namespace Client
{
    internal class Program
    {
        private static GrpcManager grpcManager;

        static void Main(string[] args)
        {
            Console.WriteLine("welcome to Manag application");

            AppLoop();
        }
        static void AppLoop()
        {
            while (true)
            {
                Console.WriteLine("enter ip address:");
                //var address = Console.ReadLine();
                var address = "localhost";


                if (string.IsNullOrEmpty(address))
                {
                    continue;
                }

                try
                {
                    grpcManager = new GrpcManager($"https://{address}:7018");

                    CommandLoop();

                    Console.WriteLine("successfull connected");
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine(ex.ToString());
                    Console.ResetColor();
                }
            }
        }
        static void CommandLoop()
        {
            var rootCommand = new RootCommand("Manag app for trolling")
            {
                new FileManagerCommand(grpcManager)
            };

            var commandLineBuilder = new CommandLineBuilder(rootCommand);

            commandLineBuilder.AddMiddleware(async (context, next) =>
            {
                try
                {
                    await next(context);

                    if (context.ParseResult.Errors.Count == 0)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.WriteLine("command send successfully");
                    }
                }
                catch (RpcException ex) when (ex.StatusCode == StatusCode.DeadlineExceeded)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("command timeout");
                }
                catch (RpcException ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine("Status code: " + ex.Status.StatusCode);
                    Console.WriteLine("Message: " + ex.Status.Detail);
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine(ex.Message);
                }
                finally
                {
                    Console.ResetColor();
                }
            });

            commandLineBuilder.UseDefaults();
            var parser = commandLineBuilder.Build();

            while (true)
            {
                Console.Write("> ");

                var command = Console.ReadLine();

                if (command is not null)
                {
                    if (command == "exit")
                    {
                        grpcManager.Dispose();
                        return;
                    }

                    var code = parser.Invoke(command);
                }
            }
        }
    }
}

//tls, interceptor, log, errors, ip list, security,