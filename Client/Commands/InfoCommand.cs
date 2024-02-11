using AngleSharp.Io;
using Client.Commands.Options;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    internal class InfoCommand : Command
    {
        private readonly GrpcManager _grpcManager;

        public InfoCommand(GrpcManager grpcManager, string name = "info", string? description = "windows information") : base(name, description)
        {
            _grpcManager = grpcManager;

            this.AddAlias("inf");

            this.AddCommand(screenCommand());
        }
        private Command screenCommand()
        {
            var command = new Command("screen", "make and save screenshot");

            var nameOption = new Option<string>(name: "--name", description: "namefile") { IsRequired = true };
            nameOption.AddAlias("-n");

            var downloadOption = new Option<bool>(name: "--download", description: "whether to download the file") { };
            downloadOption.AddAlias("-d");

            command.AddOption(nameOption);
            command.AddOption(downloadOption);

            command.SetHandler(async (name, download) =>
            {
                var request = new ScreenRequest()
                {
                    Filename = name,
                    Download = download
                };

                var deadline = 5;

                var call = _grpcManager.InfoClient.GetScreenAsync(request, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

                if(download is true)
                {
                    byte[] content = response.Content.ToByteArray();

                    var path = Path.Combine(Directory.GetCurrentDirectory(), request.Filename);

                    File.WriteAllBytes(path, content);

                    var processInfo = new ProcessStartInfo(path);
                    processInfo.UseShellExecute = true;

                    System.Diagnostics.Process.Start(processInfo);
                }

            }, nameOption, downloadOption);

            return command;
        }
    }
}