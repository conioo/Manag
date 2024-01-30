using Common.Configuration;
using Common.Helpers;
using Common.Types;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using System.Runtime.InteropServices;

namespace Server.Services
{
    public class WindowsSettingsService : WindowsSettings.WindowsSettingsBase
    {
        public WindowsSettingsService(ILogger<WindowsSettingsService> logger, IOptions<ApplicationOptions> options)
        {
            _appSettings = options.Value;
            _logger = logger;
        }

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SystemParametersInfo(int uiAction, uint uiParam, IntPtr pvParam, uint fWinIni);

        [DllImport("user32.dll", SetLastError = true)]
        static extern IntPtr CreateCursor(int width, int height);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyCursor(IntPtr hCursor);

        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;
        private const int SPI_SETMOUSE = 0x0004;
        private const int SPI_SETCURSOR = 0x0057;
        private readonly ApplicationOptions _appSettings;
        private readonly ILogger<WindowsSettingsService> _logger;

        public override Task<Empty> ChangeWallpaper(WallpaperRequest request, ServerCallContext context)
        {
            if (FileHelper.GetFileType(request.Filename) != FileType.Image)
            {
                throw new RpcException(new Status(StatusCode.Internal, "file isn't a image"));
            }

            string path = Path.Combine(_appSettings.AppFolder, _appSettings.ImagesPath, request.Filename);

            if (!File.Exists(path))
            {
                throw new RpcException(new Status(StatusCode.Internal, "file does't exist"));
            }

            int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            if (result != 0)
            {
                _logger.LogInformation("Wallpaper changed successfully");
            }
            else
            {
                _logger.LogInformation("Wallpaper doesn't changed");
            }

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> ChangeMouseSize(MouseRequest request, ServerCallContext context)
        {
            //[System.Windows.Forms.SystemInformation]::Mouse = $systemParameters

            //SystemParametersInfo(SPI_SETMOUSE, 0, IntPtr.Zero, SPIF_SENDCHANGE);

            //IntPtr newCursor = CreateCursor(request.MouseSize, request.MouseSize);

            //SystemParametersInfo(SPI_SETCURSORS, 0, newCursor, SPIF_SENDCHANGE);

            //DestroyCursor(newCursor);

            SystemParametersInfo(SPI_SETMOUSE, 0, IntPtr.Zero, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            // Tworzymy nowy kursor o żądanym rozmiarze
            IntPtr newCursor = CreateCursor(request.MouseSize, request.MouseSize);

            // Ustawiamy nowy kursor przy użyciu SystemParametersInfo
            SystemParametersInfo(SPI_SETCURSOR, 0, newCursor, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            // Zwolniamy zasoby (pamięć)
            DestroyCursor(newCursor);

            return Task.FromResult(new Empty());
        }
    }
}