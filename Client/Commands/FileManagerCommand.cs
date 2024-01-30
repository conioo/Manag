using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.CommandLine;
using Command = System.CommandLine.Command;

namespace Client.Commands
{
    internal class FileManagerCommand : Command
    {
        private readonly GrpcManager _grpcManager;

        private ByteString ConvertToByteString(FileInfo fileInfo)
        {
            byte[] data = File.ReadAllBytes(fileInfo.FullName);
            return ByteString.CopyFrom(data);
        }
        public FileManagerCommand(GrpcManager grpcManager, string name = "filemanager", string? description = "management files") : base(name, description)
        {
            _grpcManager = grpcManager;

            this.AddAlias("fm");
            this.AddCommand(addCommand());
            this.AddCommand(removeCommand());
            this.AddCommand(getCommand());
        }

        private Command addCommand()
        {
            var command = new Command("add", "adding a new file");

            var fileOption = new Option<FileInfo>(name: "--file", description: "path to file") { IsRequired = true};
            fileOption.AddAlias("-f");
            var filenameOption = new Option<string>(name: "--filename", description: "filename") { };
            filenameOption.AddAlias("-fn");

            command.AddOption(fileOption);
            command.AddOption(filenameOption);

            command.SetHandler(async (fileInfo, filename) =>
            {
                if (String.IsNullOrEmpty(filename))
                {
                    filename = fileInfo.Name;
                }

                var fileRequest = new FileRequest()
                { Filename = filename, Content = ConvertToByteString(fileInfo) };

                var call = _grpcManager.FileManagerClient.SaveFileAsync(fileRequest, deadline: DateTime.UtcNow.AddSeconds(10));

                var response = await call;

            }, fileOption, filenameOption);

            return command;
        }

        private Command removeCommand()
        {
            var command = new Command("remove", "removing a file");

            var filenameOption = new Option<string>(name: "--filename", description: "filename") { IsRequired = true };
            filenameOption.AddAlias("-fn");

            command.AddOption(filenameOption);

            command.SetHandler(async (filename) =>
            {
                var removeFileRequest = new RemoveFileRequest()
                { Filename = filename };

                var call = _grpcManager.FileManagerClient.RemoveFileAsync(removeFileRequest, deadline: DateTime.UtcNow.AddSeconds(5));

                var response = await call;

            }, filenameOption);

            return command;
        }

        private Command getCommand()
        {
            var command = new Command("get", "returns a list of files");


            command.SetHandler(async (context) =>
            {
                var call = _grpcManager.FileManagerClient.GetFilesAsync(new Empty(), deadline: DateTime.UtcNow.AddSeconds(5));

                var response = await call;

                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine("audio: ");
                Console.ResetColor();

                if (response.AudioFilenames.Count == 0)
                {
                    context.Console.WriteLine("no files found");
                }
                else
                {
                    foreach (var filename in response.AudioFilenames)
                    {
                        context.Console.WriteLine($"{filename}");
                    }
                }

                Console.ForegroundColor = ConsoleColor.Blue;
                Console.WriteLine("images: ");
                Console.ResetColor();

                if (response.ImagesFilenames.Count == 0)
                {
                    context.Console.WriteLine("no files found");
                }
                else
                {
                    foreach (var filename in response.ImagesFilenames)
                    {
                        context.Console.WriteLine($"{filename}");
                    }
                }
            });

            return command;
        }
    }
}