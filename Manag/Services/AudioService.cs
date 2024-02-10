using Common.Configuration;
using Common.Helpers;
using Google.Protobuf;
using Google.Protobuf.WellKnownTypes;
using Grpc.Core;
using Manag.Utilities;
using Microsoft.Extensions.Options;
using NAudio.Wave;
using System.Media;

namespace Manag.Services
{
    public class AudioService : Audio.AudioBase
    {
        private readonly ApplicationOptions _appOptions;
        public AudioService(IOptions<ApplicationOptions> options)
        {
            _appOptions = options.Value;
        }
        public override Task<Empty> ChangeVolume(VolumeRequest request, ServerCallContext context)
        {
            AudioHelper.ChangeVolume(request.Volume);

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Mute(MuteRequest request, ServerCallContext context)
        {
            AudioHelper.Mute();
            return Task.FromResult(new Empty());
        }

        public override Task<Empty> SystemPlay(SystemPlayRequest request, ServerCallContext context)
        {
            AudioHelper.ChangeVolume(request.Volume);

            switch (request.Type)
            {
                case SystemPlayRequest.Types.SystemSound.Beep:
                    playSystemAudio(request, () => { SystemSounds.Beep.Play(); });
                    break;
                case SystemPlayRequest.Types.SystemSound.Hand:
                    playSystemAudio(request, () => { SystemSounds.Hand.Play(); });
                    break;
            }

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Play(PlayRequest request, ServerCallContext context)
        {
            AudioHelper.ChangeVolume(request.Volume);

            _ = playAudio(Path.Combine(_appOptions.AppFolder, _appOptions.AudioPath, request.Filename));

            return Task.FromResult(new Empty());
        }

        public override Task<Empty> Record(RecordRequest request, ServerCallContext context)
        {
            var path = Path.Combine(_appOptions.AppFolder, _appOptions.AudioPath);

            FileHelper.CheckFileExist(path, request.Name);

            var recorder = new Recorder(path, request.Name);

            recorder.StartRecording();

            Thread.Sleep(request.Time * 1000);

            recorder.EndRecording();

            return Task.FromResult(new Empty());
        }

        private async Task playAudio(string path)
        {
            using (var audioFile = new AudioFileReader(path))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();

                while (outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    Thread.Sleep(1000);
                }
            }
        }
        private void playSystemAudio(SystemPlayRequest request, Action play)
        {
            for (int i = 0; i < request.Count; i++)
            {
                play();
                Thread.Sleep(request.Time * 1000);
            }
        }
    }
}