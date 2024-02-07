using System.CommandLine;

namespace Client.Commands.Options
{
    internal class DelayOption : Option<int>
    {
        public DelayOption(string name = "--delay", string? description = "delay in seconds after which the command will be executed") : base(name, description)
        {
            this.AddAlias("-d");
        }
    }
}
