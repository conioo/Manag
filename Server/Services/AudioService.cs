using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using NAudio.Wave;
using Server.Helpers;

namespace Server.Services
{
    public class AudioService : Audio.AudioBase
    {
        public async override Task<Empty> ChangeVolume(VolumeRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);

            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.ChangeVolumeAsync(request);
        }

        public async override Task<Empty> Mute(MuteRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);

            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.MuteAsync(request);
        }
        //static WaveFileWriter writer;
        public async override Task<Empty> Play(PlayRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);
           
            //var channel = GrpcHelper.CreateChannel();

            //var client = new Audio.AudioClient(channel);

            //foreach (var na in AsioOut.GetDriverNames())
            //{
            //    Console.WriteLine(na);
            //}

            //var asioOut = new AsioOut(AsioOut.GetDriverNames()[0]);

            //asioOut.InputChannelOffset = 4;
            //var recordChannelCount = 2;
            //var sampleRate = 44100;
            //asioOut.InitRecordAndPlayback(null, recordChannelCount, sampleRate);

            //asioOut.AudioAvailable += OnAsioOutAudioAvailable;

            //WaveFormat waveFormat = new WaveFormat(44100, 16, 1);

            //// Utwórz obiekt do zapisu do pliku WAV
            //writer = new WaveFileWriter("output.wav", waveFormat);

            //asioOut.Play(); // start recording
            //Thread.Sleep(5000);
            //asioOut.Stop();

            //asioOut.Dispose();
            //writer.Close();

            //void OnAsioOutAudioAvailable(object sender, AsioAudioAvailableEventArgs e)
            //{
            //    var samples = e.GetAsInterleavedSamples();
            //    writer.WriteSamples(samples, 0, samples.Length);
            //}

            return new Empty();
            //return await client.PlayAsync(request);
        }

        public async override Task<Empty> SystemPlay(SystemPlayRequest request, ServerCallContext context)
        {
            ServiceHelper.Delay(request.Delay);

            var channel = GrpcHelper.CreateChannel();

            var client = new Audio.AudioClient(channel);

            return await client.SystemPlayAsync(request);
        }
    }
}
