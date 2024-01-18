using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Proto;
using System.IO;
using System.Runtime.InteropServices;

namespace Server.Services
{
    public class WindowsSettingsService: WindowsSettings.WindowsSettingsBase
    {
        // Importuje funkcję SystemParametersInfo z user32.dll
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        public static extern int SystemParametersInfo(int uAction, int uParam, string lpvParam, int fuWinIni);

        // Definiuje stałe używane w SystemParametersInfo
        private const int SPI_SETDESKWALLPAPER = 0x0014;
        private const int SPIF_UPDATEINIFILE = 0x01;
        private const int SPIF_SENDCHANGE = 0x02;

        public override Task<Empty> ChangeWallpaper(WallpaperRequest request, ServerCallContext context)
        {
            //request.Data

            string path = @"C:\Users\posce\Desktop\logo2.png";

            int result = SystemParametersInfo(SPI_SETDESKWALLPAPER, 0, path, SPIF_UPDATEINIFILE | SPIF_SENDCHANGE);

            // Sprawdza, czy operacja się powiodła
            if (result != 0)
            {
                Console.WriteLine("Tapeta zmieniona pomyślnie.");
            }
            else
            {
                Console.WriteLine("Nie udało się zmienić tapety. Sprawdź, czy ścieżka do pliku jest poprawna.");
            }

            return base.ChangeWallpaper(request, context);
        }
    }
}
        