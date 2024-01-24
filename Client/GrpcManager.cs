using Google.Protobuf;
using Grpc.Health.V1;
using Grpc.Net.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static Google.Protobuf.FileManager;
using static Google.Protobuf.Info;
using static Google.Protobuf.WindowsSettings;

namespace Client
{
    internal class GrpcManager : IDisposable
    {
        private GrpcChannel _channel;

        private WindowsSettingsClient? _windowsSettingsClient;

        internal WindowsSettingsClient WindowsSettingsClient
        {
            get
            {
                if (_windowsSettingsClient == null)
                {
                    _windowsSettingsClient = new WindowsSettingsClient(_channel);
                }

                return _windowsSettingsClient;
            }

            private set { _windowsSettingsClient = value; }
        }

        private FileManagerClient? _fileManagerClient;

        internal FileManagerClient FileManagerClient
        {
            get
            {
                if (_fileManagerClient == null)
                {
                    _fileManagerClient = new FileManagerClient(_channel);
                }

                return _fileManagerClient;
            }

            private set { _fileManagerClient = value; }
        }

        private InfoClient? _infoClient;

        internal InfoClient InfoClient
        {
            get
            {
                if (_infoClient == null)
                {
                    _infoClient = new InfoClient(_channel);
                }

                return _infoClient;
            }

            private set { _infoClient = value; }
        }

        internal GrpcManager(string address)
        {
            this._channel = GrpcChannel.ForAddress(address);

            var response = InfoClient.CheckHealth(new Google.Protobuf.WellKnownTypes.Empty());

            if(response.Status is false)
            {
                throw new Exception("invalid connection");
            }
        }

        public void Dispose()
        {
            _channel.Dispose();
        }
    }


    //byte[] fileContent = File.ReadAllBytes("logo2.png");

    //var request = new FileRequest
    //{
    //    FileName = "logo2.png",
    //    Content = ByteString.CopyFrom(fileContent)
    //};

    //await client.SaveFileAsync(request);
    //await client2.ChangeWallpaperAsync(new WallpaperRequest { FileName = "pob.jfif" });

    //var reply = await client.SayHelloAsync(
    //                  new HelloRequest { Name = "GreeterClient" });
    //Console.WriteLine("Greeting: " + reply.Message);
    //Console.WriteLine("Press any key to exit...");

    //var reply2 = await client2.SendAsync(
    //                  new HelloRequest { Name = "GreeterClient" });
    //Console.WriteLine(reply2.Message);
    //Console.ReadKey();

}
