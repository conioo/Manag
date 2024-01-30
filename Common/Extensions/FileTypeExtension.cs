using Grpc.Core;
using Common.Configuration;
using Common.Types;

namespace Common.Extensions
{
    public static class FileTypeExtension
    {
        public static string GetFolderName(this FileType fileType, ApplicationOptions options)
        {
            if (fileType == FileType.Image)
            {
                return options.ImagesPath;
            }
            else if(fileType == FileType.Audio)
            {
                return options.AudioPath;
            }
            
            throw new RpcException(new Status(StatusCode.Internal, "undefinied file type"));
        }
    }
}
