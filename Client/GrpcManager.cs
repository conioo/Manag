using Grpc.Net.Client;
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

            if (response.Status is false)
            {
                throw new Exception("invalid connection");
            }

            InfoClient.StartSession(new Google.Protobuf.WellKnownTypes.Empty());
        }

        public void Dispose()
        {
            InfoClient.EndSession(new Google.Protobuf.WellKnownTypes.Empty());
            _channel.Dispose();
        }
    }
}
