using Client.Commands.Options;
using Client.Helpers;
using Google.Protobuf;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Client.Commands
{
    internal class AudioCommand : Command
    {
        private readonly GrpcManager _grpcManager;

        public AudioCommand(GrpcManager grpcManager, string name = "audio", string? description = "management audio in windows") : base(name, description)
        {
            _grpcManager = grpcManager;


            this.AddCommand(volumeCommand());
            this.AddCommand(muteCommand());
            this.AddCommand(systemCommand());
            this.AddCommand(playCommand());
        }

        private Command volumeCommand()
        {
            var command = new Command("volume", "change volume");

            var volumeOption = new Option<int>(name: "--volume", description: "system volume, range: 0-100") {IsRequired = true };
            volumeOption.AddAlias("-v");

            volumeOption.AddValidator(Validators.Validators.VolumeValidator(volumeOption));

            var delayOption = new DelayOption();

            command.AddOption(volumeOption);
            command.AddOption(delayOption);

            command.SetHandler(async (volume, delay) =>
            {
                var volumeRequest = new VolumeRequest()
                {
                    Volume = volume,
                    Delay = delay
                };

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.AudioClient.ChangeVolumeAsync(volumeRequest, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

            }, volumeOption, delayOption);

            return command;
        }

        private Command muteCommand()
        {
            var command = new Command("mute", "mute audio");

            var delayOption = new DelayOption();

            command.AddOption(delayOption);

            command.SetHandler(async (delay) =>
            {
                var muteRequest = new MuteRequest()
                {
                    Delay = delay
                };

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.AudioClient.MuteAsync(muteRequest, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

            }, delayOption);

            return command;
        }

        private Command systemCommand()
        {
            var command = new Command("system", "play system voice");

            var volumeOption = new Option<int>(name: "--volume", description: "system volume, range: 0-100") { IsRequired = true };
            volumeOption.AddAlias("-v");

            var countOption = new Option<int>(name: "--count", description: "number of playing system voice", getDefaultValue: () => { return 1; }) { };
            countOption.AddAlias("-c");

            var timeOption = new Option<int>(name: "--time", description: "time between playing system voice in seconds", getDefaultValue: () => { return 3; }) { };
            timeOption.AddAlias("-t");

            var typeOption = new Option<string>(name: "--type", description: "windows auido type choose: beep, hand") { IsRequired = true };
            typeOption.AddAlias("-typ");
            volumeOption.AddValidator(Validators.Validators.VolumeValidator(volumeOption));

            var delayOption = new DelayOption();

            command.AddOption(volumeOption);
            command.AddOption(delayOption);
            command.AddOption(countOption);
            command.AddOption(timeOption);
            command.AddOption(typeOption);

            command.SetHandler(async (count, time, type, volume, delay) =>
            {
                var systemPlayRequest = new SystemPlayRequest()
                {
                    Count = count,
                    Volume = volume,
                    Delay = delay,
                    Time = time,
                };

                if(type == "beep")
                {
                    systemPlayRequest.Type = SystemPlayRequest.Types.SystemSound.Beep;
                }
                else if(type == "hand")
                {
                    systemPlayRequest.Type = SystemPlayRequest.Types.SystemSound.Hand;
                }
                else
                {
                    ConsoleHelper.WriteError("incorrect system sounds type");
                    return;
                }

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.AudioClient.SystemPlayAsync(systemPlayRequest, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

            }, countOption, timeOption, typeOption, volumeOption, delayOption);

            return command;
        }

        private Command playCommand()
        {
            var command = new Command("play", "play mp3 audio file");

            var volumeOption = new Option<int>(name: "--volume", description: "system volume, range: 0-100") { IsRequired = true };
            volumeOption.AddAlias("-v");

            var nameOption = new Option<string>(name: "--name", description: "filename audio name") { IsRequired = true };
            nameOption.AddAlias("-n");

            volumeOption.AddValidator(Validators.Validators.VolumeValidator(volumeOption));

            var delayOption = new DelayOption();

            command.AddOption(volumeOption);
            command.AddOption(nameOption);
            command.AddOption(delayOption);

            command.SetHandler(async (name, volume, delay) =>
            {
                var playRequest = new PlayRequest()
                {
                    Volume = volume,
                    Delay = delay,
                    Filename = name
                };

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.AudioClient.PlayAsync(playRequest, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

            }, nameOption, volumeOption, delayOption);

            return command;
        }
    }
}
