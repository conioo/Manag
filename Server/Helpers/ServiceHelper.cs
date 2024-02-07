using AngleSharp.Io;

namespace Server.Helpers
{
    internal static class ServiceHelper
    {
        internal static void Delay(int? delay)
        {
            if (delay is not null)
            {
                Thread.Sleep((int)delay * 1000);
            }
        }
    }
}
