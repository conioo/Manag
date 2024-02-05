using System.CommandLine;
using System.CommandLine.Parsing;

namespace Client.Commands.Validators
{
    internal static class Validators
    {
        internal static ValidateSymbolResult<OptionResult> VolumeValidator(Option<int> volumeOption)
        {
            return optionResult =>
            {
                var volume = optionResult.GetValueForOption(volumeOption);

                if (volume < 0 || volume > 100)
                {
                    optionResult.ErrorMessage = "volume must be in 0-100 range";
                }
            };
        }
    }
}
