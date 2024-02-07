using Client.Commands.Options;
using Google.Protobuf;
using System.CommandLine;
using Command = System.CommandLine.Command;

namespace Client.Commands
{
    internal class SettingsCommand : Command
    {
        private readonly GrpcManager _grpcManager;

        public SettingsCommand(GrpcManager grpcManager, string name = "settings", string? description = "management windows settings options") : base(name, description)
        {
            _grpcManager = grpcManager;

            this.AddAlias("st");

            this.AddCommand(changeWallpaperCommand());
            this.AddCommand(changeMouseSizeCommand());
        }

        private Command changeWallpaperCommand()
        {
            var command = new Command("wallpaper", "change wallpaper");

            var filenameOption = new Option<string>(name: "--filename", description: "filename") { IsRequired = true };
            filenameOption.AddAlias("-fn");

            command.AddOption(filenameOption);

            var delayOption = new DelayOption();
            command.AddOption(delayOption);

            command.SetHandler(async (filename, delay) =>
            {

                var wallpaperRequest = new WallpaperRequest()
                { Filename = filename, Delay = delay};

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.WindowsSettingsClient.ChangeWallpaperAsync(wallpaperRequest, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

            }, filenameOption, delayOption);

            return command;
        }

        private Command changeMouseSizeCommand()
        {
            var command = new Command("mousesize", "change mouse size");

            var sizeOption = new Option<int>(name: "--size", description: "new size for mouse") { IsRequired = true };

            command.AddOption(sizeOption);

            command.SetHandler(async (size) =>
            {
                var mouseRequest = new MouseRequest()
                { MouseSize = size };

                var call = _grpcManager.WindowsSettingsClient.ChangeMouseSizeAsync(mouseRequest, deadline: DateTime.UtcNow.AddSeconds(5));

                var response = await call;

            }, sizeOption);

            return command;
        }
    }
}