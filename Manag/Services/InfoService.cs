using Common.Configuration;
using Common.Helpers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace Manag.Services
{
    public class InfoService : Info.InfoBase
    {
        public InfoService(IOptions<ApplicationOptions> options)
        {
            _appOptions = options.Value;
        }
        public override Task<HealthResponse> CheckHealth(Empty request, ServerCallContext context)
        {
            var response = new HealthResponse() { Status = true };

            return Task.FromResult(response);
        }

        [DllImport("user32.dll")]
        public static extern IntPtr GetDC(IntPtr hwnd);

        [DllImport("gdi32.dll")]
        public static extern int GetDeviceCaps(IntPtr hdc, int nIndex);

        public const int DESKTOPVERTRES = 117;
        public const int DESKTOPHORZRES = 118;
        private readonly ApplicationOptions _appOptions;

        public override Task<ScreenResponse> GetScreen(ScreenRequest request, ServerCallContext context)
        {
            var path = Path.Combine(_appOptions.AppFolder, _appOptions.ImagesPath);

            FileHelper.CheckFileExist(path, request.Filename);

            IntPtr hdc = GetDC(IntPtr.Zero);

            int screenWidth = GetDeviceCaps(hdc, DESKTOPHORZRES);
            int screenHeight = GetDeviceCaps(hdc, DESKTOPVERTRES);

            int captureX = 0;
            int captureY = 0;

            Bitmap bitmap = new Bitmap(screenWidth, screenHeight);

            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(captureX, captureY, 0, 0, bitmap.Size);
            }

            bitmap.Save(Path.Combine(path, request.Filename), ImageFormat.Png);

            var response = new ScreenResponse();

            if(request.Download)
            {
                byte[] imageBytes;
                
                using (MemoryStream stream = new MemoryStream())
                {
                    bitmap.Save(stream, ImageFormat.Png);
                    imageBytes = stream.ToArray();
                }

                response.Content = ByteString.CopyFrom(imageBytes);
            }

            return Task.FromResult(response);
        }
    }
}