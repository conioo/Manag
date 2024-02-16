using Common.Configuration;
using Common.Extensions;
using Common.Helpers;
using Common.Interfaces;
using Google.Protobuf;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Drawing;

namespace Common.Services
{
    public class FileService : IFileService
    {
        private readonly ApplicationOptions _appOptions;
        private readonly ILogger<FileService> _logger;

        public FileService(ILogger<FileService> logger, IOptions<ApplicationOptions> options)
        {
            _appOptions = options.Value;
            _logger = logger;
        }

        public void SaveFile(string filename, ByteString content)
        {
            try
            {
                var path = Path.Combine(_appOptions.AppFolder, FileHelper.GetFileType(filename).GetFolderName(_appOptions));

                FileHelper.CheckPath(path);

                if (File.Exists(Path.Combine(path, filename)))
                {
                    _logger.LogInformation($"file {filename} already exist");

                    throw new RpcException(new Status(StatusCode.Internal, $"file {filename} already exist"));
                }

                byte[] byteArray = content.ToByteArray();

                File.WriteAllBytes(Path.Combine(path, filename), byteArray);
                _logger.LogInformation($"successfull created file {filename}");
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
        }

        public void SaveFile(string filename, Bitmap content)
        {
            try
            {
                var path = Path.Combine(_appOptions.AppFolder, FileHelper.GetFileType(filename).GetFolderName(_appOptions));

                FileHelper.CheckPath(path);

                if (File.Exists(Path.Combine(path, filename)))
                {
                    _logger.LogInformation($"file {filename} already exist");

                    throw new RpcException(new Status(StatusCode.Internal, $"file {filename} already exist"));
                }

                content.Save(Path.Combine(path, filename), System.Drawing.Imaging.ImageFormat.Png);

                _logger.LogInformation($"successfull created file {filename}");
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
        }
    }
}
