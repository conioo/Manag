using Client.Commands.Options;
using Client.Helpers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using System.CommandLine;
using Command = System.CommandLine.Command;

namespace Client.Commands
{
    internal class ProcessCommand : Command
    {
        private readonly GrpcManager _grpcManager;

        public ProcessCommand(GrpcManager grpcManager, string name = "process", string? description = "management processes in windows") : base(name, description)
        {
            _grpcManager = grpcManager;

            this.AddAlias("pr");

            this.AddCommand(startProcessCommand());
            this.AddCommand(startIntervalProcessCommand());
            this.AddCommand(getCommand());
            this.AddCommand(killCommand());
            this.AddCommand(winCommand());

            //this.AddValidator(symbolResult =>
            //{
            //    var optionValue = symbolResult.Children.get.GetByAlias("--my-option")?.GetValueOrDefault<int>() ?? 0;

            //    if (optionValue < 0 || optionValue > 100)
            //    {
            //        return $"Option value must be within the range of 0 to 100.";
            //    }

            //    return null;
            //});
        }

        private Command startProcessCommand()
        {
            var command = new Command("start", "start a new process");

            var processNameOption = new Option<string>(name: "--name", description: "name of process") { IsRequired = true };
            processNameOption.AddAlias("-n");

            var argumentOption = new Option<string>(name: "--argument", description: "optional argument for new process") { };
            argumentOption.AddAlias("-arg");

            var countOption = new Option<int>(name: "--count", description: "number of new processes", getDefaultValue: () => 1) { };
            countOption.AddAlias("-c");

            var volumeOption = new Option<int>(name: "--volume", description: "optional system volume, range: 0-100") { };
            volumeOption.AddAlias("-v");

            var delayOption = new DelayOption();

            volumeOption.AddValidator(Validators.Validators.VolumeValidator(volumeOption));

            command.AddOption(countOption);
            command.AddOption(argumentOption);
            command.AddOption(processNameOption);
            command.AddOption(volumeOption);
            command.AddOption(delayOption);

            command.SetHandler(async (name, argument, count, volume, delay) =>
            {
                var processRequest = new ProcessRequest()
                {
                    ProcessName = name,
                    Count = count,
                    Delay = delay
                };

                if (volume != 0)
                {
                    processRequest.Volume = volume;
                }

                if (argument is not null)
                {
                    processRequest.Arguments = argument;
                }
                var deadline = 5;
                if(delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.ProcessClient.NewProcessAsync(processRequest, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

            }, processNameOption, argumentOption, countOption, volumeOption, delayOption);

            return command;
        }

        private Command startIntervalProcessCommand()
        {
            var command = new Command("interval", "start a new interval process");

            var processNameOption = new Option<string>(name: "--name", description: "name of process") { IsRequired = true };
            processNameOption.AddAlias("-n");

            var argumentOption = new Option<string>(name: "--argument", description: "optional argument for new process") { };
            argumentOption.AddAlias("-arg");

            var intervalsOption = new Option<int>(name: "--count", description: "number of the new processes by interval", getDefaultValue: () => 1) { };
            intervalsOption.AddAlias("-c");

            var countOption = new Option<int>(name: "--intervals", description: "number of the intervals", getDefaultValue: () => 5) { };
            countOption.AddAlias("-in");

            var timeOption = new Option<int>(name: "--time", description: "interval time in seconds", getDefaultValue: () => 5) { };
            timeOption.AddAlias("-t");

            var volumeOption = new Option<int>(name: "--volume", description: "optional system volume, range: 0-100") { };
            volumeOption.AddAlias("-v");
            volumeOption.AddValidator(Validators.Validators.VolumeValidator(volumeOption));

            var delayOption = new DelayOption();
            command.AddOption(delayOption);

            command.AddOption(countOption);
            command.AddOption(argumentOption);
            command.AddOption(processNameOption);
            command.AddOption(timeOption);
            command.AddOption(intervalsOption);
            command.AddOption(volumeOption);

            command.SetHandler(async (name, argument, countPerInterval, count, time, volume, delay) =>
            {
                var intervalRequest = new IntervalRequest()
                {
                    ProcessName = name,
                    CountPerInterval = countPerInterval,
                    Count = count,
                    IntervalInSeconds = time,
                    Delay = delay
                };

                if (volume != 0)
                {
                    intervalRequest.Volume = volume;
                }

                if (argument is not null)
                {
                    intervalRequest.Arguments = argument;
                }

                var call = _grpcManager.ProcessClient.NewIntervalProcessAsync(intervalRequest);

                var response = await call;

            }, processNameOption, argumentOption, intervalsOption, countOption, timeOption, volumeOption, delayOption);

            return command;
        }

        private Command getCommand()
        {
            var command = new Command("get", "get processes");

            command.SetHandler(async () =>
            {
                var call = _grpcManager.ProcessClient.GetProcessesAsync(new Empty(), deadline: DateTime.UtcNow.AddSeconds(5));

                var response = await call;

                foreach (var processName in response.ProcessNames)
                {
                    Console.Write(processName + " ");
                }
            });

            return command;
        }

        private Command killCommand()
        {
            var command = new Command("kill", "kill process");

            var processNameOption = new Option<string>(name: "--name", description: "name of process") { };
            processNameOption.AddAlias("-n");

            var allOption = new Option<bool>(name: "--all", description: "kill all processes");
            allOption.AddAlias("-a");

            var delayOption = new DelayOption();
            command.AddOption(delayOption);

            command.AddOption(processNameOption);
            command.AddOption(allOption);

            command.SetHandler(async (name, all, delay) =>
            {
                if ((name is null && all is false) || (name is not null && all is true))
                {
                    ConsoleHelper.WriteError("only can be selected name or all option");
                    return;
                }

                var processRequest = new ShutdownRequest()
                {
                    IsAll = all,
                    Delay = delay
                };

                if (name is not null)
                {
                    processRequest.ProcessName = name;
                }

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.ProcessClient.ShutdownProcessAsync(processRequest, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

                Console.WriteLine($"shutdown processes: {response.CountShutdownProcesses}");

            }, processNameOption, allOption, delayOption);

            return command;
        }

        private Command winCommand()
        {
            var command = new Command("win", "shutdown a system");

            var delayOption = new DelayOption();
            command.AddOption(delayOption);

            command.SetHandler(async (delay) =>
            {
                var request = new ShutdownWindowsRequest() { Delay = delay };

                var deadline = 5;
                if (delay != null)
                {
                    deadline += delay;
                }

                var call = _grpcManager.ProcessClient.ShutdownWindowsAsync(request, deadline: DateTime.UtcNow.AddSeconds(deadline));

                var response = await call;

            }, delayOption);

            return command;
        }
    }
}