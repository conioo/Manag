using AngleSharp.Io;
using Common.Types;
using Grpc.Core;

namespace Common.Helpers
{
    public static class FileHelper
    {
        static FileHelper()
        {
            
        }
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

        public static void CheckFileExist(string path, string filename)
        {
            if (File.Exists(Path.Combine(path, filename)))
            {
                throw new RpcException(new Status(StatusCode.Internal, $"file {filename} already exist"));
            }
        }

        public static void CheckFileExist(string path)
        {
            CheckFileExist(Path.GetFileName(path), Path.GetDirectoryName(path));
        }

        public static void CheckPath(string path)
        {
            if (!Directory.Exists(path))
            {
                try
                {
                    Directory.CreateDirectory(path);
                }
                catch (Exception ex)
                {
                    throw new RpcException(new Status(StatusCode.Internal, ex.Message));
                }
            }
        }
    }
}