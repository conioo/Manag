using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Microsoft.Extensions.Options;
using Server.Configuration;

namespace Server.Services
{
    internal class FileManagerService : FileManager.FileManagerBase
    {
        private readonly ApplicationOptions _appOptions;
        private readonly ILogger<FileManagerService> _logger;

        public FileManagerService(IOptions<ApplicationOptions> appOptions, ILogger<FileManagerService> logger)
        {
            Console.WriteLine(appOptions.Value.FilesPath);
            _appOptions = appOptions.Value;
            _logger = logger;
        }

        public override Task<Empty> SaveFile(FileRequest request, ServerCallContext context)
        {
            checkPath();

            if (File.Exists(Path.Combine(_appOptions.FilesPath, request.Filename)))
            {
                _logger.LogInformation($"file {request.Filename} already exist");

                throw new RpcException(new Status(StatusCode.Internal, $"file {request.Filename} already exist"));
            }

            byte[] content = request.Content.ToByteArray();

            try
            {
                File.WriteAllBytes(Path.Combine(_appOptions.FilesPath, request.Filename), content);
                _logger.LogInformation($"successfull created file {request.Filename}");
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
            checkPath();

            if (!File.Exists(Path.Combine(_appOptions.FilesPath, request.Filename)))
            {
                _logger.LogInformation($"file {request.Filename} doesn't exist");

                throw new RpcException(new Status(StatusCode.Internal, $"file {request.Filename} doesn't exist"));
            }

            try
            {
                File.Delete(Path.Combine(_appOptions.FilesPath, request.Filename));

                _logger.LogInformation($"successfull removed file {request.Filename}");
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
            checkPath();

            try
            {
                var files = Directory.GetFiles(_appOptions.FilesPath);

                var filenames = new List<string>();

                foreach (var file in files)
                {
                    filenames.Add(Path.GetFileName(file));
                }

                var response = new FilesResponse() { Filenames = { filenames } };

                return Task.FromResult(response);
            }
            catch (Exception ex)
            {
                _logger.LogError($"error during downloading filenames {ex.Message}");
                throw new RpcException(new Status(StatusCode.Internal, ex.Message));
            }
        }

        private void checkPath()
        {
            if (!Directory.Exists(_appOptions.FilesPath))
            {
                _logger.LogInformation("directory doesn't exist");
                try
                {
                    Directory.CreateDirectory(_appOptions.FilesPath);
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