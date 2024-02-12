using Client.Commands.Options;
using Google.Protobuf;
using System.CommandLine;

namespace Client.Commands
{
    internal class MessageCommand : Command
    {
        private readonly GrpcManager _grpcManager;

        public MessageCommand(GrpcManager grpcManager, string name = "message", string? description = "show messageBox") : base(name, description)
        {
            _grpcManager = grpcManager;

            this.AddAlias("msg");

            this.AddCommand(errorCommand());
            this.AddCommand(noticeCommand());
            this.AddCommand(intervalCommand());
        }

        private Command errorCommand()
        {
            var command = new Command("error", "show error messageBox");

            var countOption = new Option<int>(name: "--count", description: "number of errors", getDefaultValue: () => { return 1; }) { };
            countOption.AddAlias("-c");

            var delayOption = new DelayOption();

            command.AddOption(countOption);
            command.AddOption(delayOption);

            command.SetHandler(async (count, delay) =>
            {
                var request = new ErrorRequest()
                {
                    Count = count,
                    Delay = delay
                };

                var call = _grpcManager.MessageClient.ShowErrorAsync(request);

                var response = await call;

            }, countOption, delayOption);

            return command;
        }

        private Command noticeCommand()
        {
            var command = new Command("notice", "send message");
            command.AddAlias("ntc");

            var countOption = new Option<int>(name: "--count", description: "number of showing message", getDefaultValue: () => { return 1; }) { };
            countOption.AddAlias("-c");

            var contentOption = new Option<string>(name: "--content", description: "message content") { IsRequired = true };
            contentOption.AddAlias("-con");

            var captionOption = new Option<string>(name: "--caption", description: "message caption") { IsRequired = true };
            captionOption.AddAlias("-capt");

            var delayOption = new DelayOption();

            command.AddOption(countOption);
            command.AddOption(contentOption);
            command.AddOption(captionOption);
            command.AddOption(delayOption);

            command.SetHandler(async (count, content, caption, delay) =>
            {
                var request = new MessageRequest()
                {
                    Count = count,
                    Delay = delay,
                    Content = content,
                    Caption = caption
                };

                var call = _grpcManager.MessageClient.ShowMessageAsync(request);

                var response = await call;

                Console.WriteLine($"response: {response.Result}");

            }, countOption, contentOption, captionOption, delayOption);

            return command;
        }

        private Command intervalCommand()
        {
            var command = new Command("interval", "show errors in intervals");

            var intervalsOption = new Option<int>(name: "--intervals", description: "number of intervals", getDefaultValue: () => { return 5; }) { };
            intervalsOption.AddAlias("-int");

            var countOption = new Option<int>(name: "--count", description: "number errors in intervals", getDefaultValue: () => { return 2; }) { };
            countOption.AddAlias("-c");

            var timeOption = new Option<int>(name: "--time", description: "interval time in seconds", getDefaultValue: () => { return 10; }) { };
            timeOption.AddAlias("-t");

            var delayOption = new DelayOption();

            command.AddOption(countOption);
            command.AddOption(delayOption);
            command.AddOption(intervalsOption);
            command.AddOption(timeOption);

            command.SetHandler(async (count, delay, intervals, time) =>
            {
                var request = new ErrorIntervalMessage()
                {
                    Count = intervals,
                    Delay = delay,
                    CountPerInterval = count,
                    IntervalInSeconds = time
                };

                var call = _grpcManager.MessageClient.ErrorIntervalAsync(request);

                var response = await call;

            }, countOption, delayOption, intervalsOption, timeOption);

            return command;
        }
    }
}