using Grpc.Net.Client;
using static Google.Protobuf.FileManager;
using static Google.Protobuf.Info;
using static Google.Protobuf.WindowsSettings;
using static Google.Protobuf.Process;
using static Google.Protobuf.Audio;
using static Google.Protobuf.Message;
using static Google.Protobuf.Camera;


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

        private ProcessClient? _processClient;

        internal ProcessClient ProcessClient
        {
            get
            {
                if (_processClient == null)
                {
                    _processClient = new ProcessClient(_channel);
                }

                return _processClient;
            }

            private set { _processClient = value; }
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

        private AudioClient? _audioClient;

        internal AudioClient AudioClient
        {
            get
            {
                if (_audioClient == null)
                {
                    _audioClient = new AudioClient(_channel);
                }

                return _audioClient;
            }

            private set { _audioClient = value; }
        }

        private MessageClient? _messageClient;

        internal MessageClient MessageClient
        {
            get
            {
                if (_messageClient == null)
                {
                    _messageClient = new MessageClient(_channel);
                }

                return _messageClient;
            }

            private set { _messageClient = value; }
        }

        private CameraClient? _cameraClient;

        internal CameraClient CameraClient
        {
            get
            {
                if (_cameraClient == null)
                {
                    _cameraClient = new CameraClient(_channel);
                }

                return _cameraClient;
            }

            private set { _cameraClient = value; }
        }

        internal GrpcManager(string address)
        {
            var channelOptions = new GrpcChannelOptions
            {
                MaxSendMessageSize = null, // 16 MB
                MaxReceiveMessageSize = null // 16 MB
            };

            this._channel = GrpcChannel.ForAddress(address, channelOptions);

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
