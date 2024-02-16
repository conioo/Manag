using Google.Protobuf;
using System.Diagnostics;

namespace Client.Helpers
{
    internal static class FileHelper
    {
        internal static void SaveFile(ByteString bytes, string filename, bool show = false)
        {
            byte[] content = bytes.ToByteArray();

            var path = Path.Combine(Directory.GetCurrentDirectory(), filename);

            File.WriteAllBytes(path, content);

            if (show)
            {
                var processInfo = new ProcessStartInfo(path);
                processInfo.UseShellExecute = true;

                System.Diagnostics.Process.Start(processInfo);
            }
        }
    }
}
