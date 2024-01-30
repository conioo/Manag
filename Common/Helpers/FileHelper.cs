using Common.Types;

namespace Common.Helpers
{
    public static class FileHelper
    {
        public static FileType GetFileType(string filename)
        {
            var extension = Path.GetExtension(filename);

            switch (extension)
            {
                case ".jpg":
                case ".jpeg":
                case ".gif":
                case ".tiff":
                case ".tif":
                case ".png":
                case ".svg":
                case ".bmp":
                case ".webp":
                case ".jfif":
                    return FileType.Image;
                case ".mp3":
                case ".wav":
                case ".flac":
                case ".wma":
                case ".aac":
                    return FileType.Audio;
                default:
                    return FileType.Undefinied;
            }
        }
    }
}