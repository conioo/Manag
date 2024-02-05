using Client.Helpers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.CommandLine;
using YoutubeExplode;
using YoutubeExplode.Videos.Streams;
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

        private async Task<ByteString> GetByteStringFromYoutube(string videoUrl)
        {
            var youtube = new YoutubeClient();

            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            return ByteString.FromStream(stream);
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

            var fileOption = new Option<FileInfo>(name: "--file", description: "path to file") { };
            fileOption.AddAlias("-f");

            var filenameOption = new Option<string>(name: "--name", description: "filename") { };
            filenameOption.AddAlias("-n");

            var linkOption = new Option<string>(name: "--link", description: "link to youtube film, save audio from youtube film, filename is required") { };
            linkOption.AddAlias("-l");

            command.AddOption(fileOption);
            command.AddOption(filenameOption);
            command.AddOption(linkOption);

            command.SetHandler(async (fileInfo, filename, link) =>
            {
                var fileRequest = new FileRequest();

                if (fileInfo is null && link is null)
                {
                    ConsoleHelper.WriteError("can't be file and link option at the same time");
                    return;
                }

                if (link is not null)
                {
                    if (String.IsNullOrEmpty(filename))
                    {
                        ConsoleHelper.WriteError("filename is required when you provide a link");
                        return;
                    }

                    fileRequest.Content = await GetByteStringFromYoutube(link);
                    fileRequest.Filename = filename;
                }
                else
                {
                    fileRequest.Content = ConvertToByteString(fileInfo);

                    if (String.IsNullOrEmpty(filename))
                    {
                        fileRequest.Filename = fileInfo.Name;
                    }
                    else
                    {
                        fileRequest.Filename = filename;
                    }
                }

                var call = _grpcManager.FileManagerClient.SaveFileAsync(fileRequest, deadline: DateTime.UtcNow.AddSeconds(10));

                var response = await call;

            }, fileOption, filenameOption, linkOption);

            return command;
        }

        private Command removeCommand()
        {
            var command = new Command("remove", "removing a file");

            var filenameOption = new Option<string>(name: "--filename", description: "filename") { IsRequired = true };
            filenameOption.AddAlias("-n");

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

