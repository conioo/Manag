using Client.Commands.Options;
using Client.Helpers;
using Google.Protobuf;
using System.CommandLine;

namespace Client.Commands
{
    internal class CameraCommand : Command
    {
        private readonly GrpcManager _grpcManager;

        public CameraCommand(GrpcManager grpcManager, string name = "camera", string? description = "management camera") : base(name, description)
        {
            _grpcManager = grpcManager;


            this.AddCommand(photoCommand());
        }

        private Command photoCommand()
        {
            var command = new Command("photo", "take one photo");

            var nameOption = new Option<string>(name: "--name", description: "filename without extension") { IsRequired = true };
            nameOption.AddAlias("-n");

            var wallOption = new Option<bool>(name: "--wall", description: "set it as wallpaper") { };
            nameOption.AddAlias("-w");

            var delayOption = new DelayOption();

            command.AddOption(delayOption);
            command.AddOption(nameOption);
            command.AddOption(wallOption);

            command.SetHandler(async (wall, name, delay) =>
            {
                var request = new PhotoRequest()
                {
                    Name = name,
                    SetWallpaper = wall,
                    Delay = delay
                };

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.CameraClient.PhotoAsync(request, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

                FileHelper.SaveFile(response.Content, request.Name);

            }, wallOption, nameOption, delayOption);

            return command;
        }
    }
}