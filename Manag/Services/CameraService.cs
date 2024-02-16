using Common.Configuration;
using Common.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Manag.Services
{
    public class CameraService : Camera.CameraBase
    {
        private readonly ApplicationOptions _appOptions;
        private readonly IFileService _fileService;

        public CameraService(IOptions<ApplicationOptions> options, IFileService fileService)
        {
            _appOptions = options.Value;
            _fileService = fileService;
        }

        public override Task<PhotoResponse> Photo(PhotoRequest request, ServerCallContext context)
        {
            var camera = new Manag.Utilities.Camera();

            var bitmap = camera.TakePhoto();

            string fileName = $"{request.Name}";

            _fileService.SaveFile(fileName, bitmap);

            byte[] imageBytes;
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Png);
                imageBytes = ms.ToArray();
            }

            ByteString byteString = ByteString.CopyFrom(imageBytes);

            var response = new PhotoResponse()
            {
                Content = byteString
            };

            return Task.FromResult(response);
        }
    }
}