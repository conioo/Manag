using Grpc.Core;
using NAudio.Wave;

namespace Manag.Utilities
{
    internal class Recorder
    {
        private WaveInEvent? _sourceStream;
        private WaveFileWriter? _waveWriter;
        private readonly string _filePath;
        private readonly string _fileName;
        private readonly int _inputDeviceIndex;

        internal Recorder(string filePath, string fileName)
        {
            if(WaveInEvent.DeviceCount == 0)
            {
                throw new RpcException(new Status(StatusCode.Internal, $"0 Device found"));
            }

            _inputDeviceIndex = 0;
            _fileName = fileName;
            _filePath = filePath;
        }

        public void StartRecording()
        {
            _sourceStream = new WaveInEvent
            {
                DeviceNumber = _inputDeviceIndex,
                WaveFormat = new WaveFormat(44100, WaveInEvent.GetCapabilities(_inputDeviceIndex).Channels)
            };

            _sourceStream.DataAvailable += SourceStreamDataAvailable;

            if (!Directory.Exists(_filePath))
            {
                Directory.CreateDirectory(_filePath);
            }

            _waveWriter = new WaveFileWriter(Path.Combine(_filePath, _fileName), _sourceStream.WaveFormat);
            _sourceStream.StartRecording();
        }

        private void SourceStreamDataAvailable(object sender, WaveInEventArgs e)
        {
            if (_waveWriter is null) return;
            _waveWriter.Write(e.Buffer, 0, e.BytesRecorded);
            _waveWriter.Flush();
        }

        public void EndRecording()
        {
            if (_sourceStream is not null)
            {
                _sourceStream.StopRecording();
                _sourceStream.Dispose();
                _sourceStream = null;
            }
            if (_waveWriter is null)
            {
                return;
            }
            _waveWriter.Dispose();
            _waveWriter = null;
        }
    }
}
