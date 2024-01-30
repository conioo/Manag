using Common.Configuration;
using Common.Helpers;
using Common.Extensions;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;

namespace Server.Services
{
    internal class FileManagerService : FileManager.FileManagerBase
    {
        private readonly ApplicationOptions _appOptions;
        private readonly ILogger<FileManagerService> _logger;

        public FileManagerService(IOptions<ApplicationOptions> appOptions, ILogger<FileManagerService> logger)
        {
            _appOptions = appOptions.Value;
            _logger = logger;
        }

        public override Task<Empty> SaveFile(FileRequest request, ServerCallContext context)
        {
            try
            {
                var path = Path.Combine(_appOptions.AppFolder, FileHelper.GetFileType(request.Filename).GetFolderName(_appOptions));

                checkPath(path);

                if (File.Exists(Path.Combine(path, request.Filename)))
                {
                    _logger.LogInformation($"file {request.Filename} already exist");

                    throw new RpcException(new Status(StatusCode.Internal, $"file {request.Filename} already exist"));
                }

                byte[] content = request.Content.ToByteArray();

                File.WriteAllBytes(Path.Combine(path, request.Filename), content);
                _logger.LogInformation($"successfull created file {request.Filename}");
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"error during creating file {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> RemoveFile(RemoveFileRequest request, ServerCallContext context)
        {
            try
            {
                var path = Path.Combine(_appOptions.AppFolder, FileHelper.GetFileType(request.Filename).GetFolderName(_appOptions));

                checkPath(path);

                if (!File.Exists(Path.Combine(path, request.Filename)))
                {
                    _logger.LogInformation($"file {request.Filename} doesn't exist");

                    throw new RpcException(new Status(StatusCode.Internal, $"file {request.Filename} doesn't exist"));
                }

                File.Delete(Path.Combine(path, request.Filename));

                _logger.LogInformation($"successfull removed file {request.Filename}");
            }
            catch (RpcException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"error during removing file {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }

            return Task.FromResult(new Empty());
        }

        public override Task<FilesResponse> GetFiles(Empty request, ServerCallContext context)
        {
            try
            {
                var imagePath = Path.Combine(_appOptions.AppFolder, _appOptions.ImagesPath);
                var audioPath = Path.Combine(_appOptions.AppFolder, _appOptions.AudioPath);

                checkPath(imagePath);
                checkPath(audioPath);

                var imageFiles = Directory.GetFiles(imagePath);
                var audioFiles = Directory.GetFiles(audioPath);

                var imageFilenames = new List<string>();
                var audioFilenames = new List<string>();


                foreach (var file in imageFiles)
                {
                    imageFilenames.Add(Path.GetFileName(file));
                }

                foreach (var file in audioFiles)
                {
                    audioFilenames.Add(Path.GetFileName(file));
                }

                var response = new FilesResponse() { AudioFilenames = { audioFilenames }, ImagesFilenames = { imageFilenames } };

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error during downloading filenames {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private void checkPath(string path)
        {
            if (!Directory.Exists(path))
            {
                _logger.LogInformation("directory doesn't exist");
                try
                {
                    Directory.CreateDirectory(path);
                    _logger.LogInformation("directory was created");

                }
                catch (Exception ex)
                {
                    _logger.LogError($"error during creating directory {ex.Message}");
                    throw new RpcException(new Status(StatusCode.Internal, ex.Message));
                }
            }
        }
    }
}