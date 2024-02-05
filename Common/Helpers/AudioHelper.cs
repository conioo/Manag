using NAudio.CoreAudioApi;
using NAudio.Wave;
using YoutubeExplode;
using YoutubeExplode.Converter;
using YoutubeExplode.Videos.Streams;

namespace Common.Helpers
{
    public static class AudioHelper
    {
        public static void changeVolume(int? volume)
        {
            if (volume is null) return;

            var audioDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);

            audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar = ((float)(volume / 100f));
            audioDevice.AudioEndpointVolume.Mute = false;
        }


        public static void changeVolume2(int volume)
        {
            var enumerator = new MMDeviceEnumerator();
            var devices = enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active);

            //foreach (var device in devices)
            //{
            //    //device.AudioEndpointVolume.Mute = false; 
            //    //device.AudioEndpointVolume.MasterVolumeLevelScalar = 0.1f;
            //    //Console.WriteLine(JsonConvert.SerializeObject(device));
            //    Console.WriteLine(device.DeviceFriendlyName + ": " + device.AudioEndpointVolume.Mute + " " + device.InstanceId + " " + device.AudioEndpointVolume.MasterVolumeLevelScalar);
            //}

            var enumerator2 = new MMDeviceEnumerator();
            foreach (var wasapi in enumerator2.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
            {
                Console.WriteLine($"{wasapi.DataFlow} {wasapi.FriendlyName} {wasapi.DeviceFriendlyName} {wasapi.ID}");
            }

            var audioDevice = new MMDeviceEnumerator().GetDefaultAudioEndpoint(DataFlow.Render, Role.Multimedia);
            Console.WriteLine($"dd {audioDevice.DataFlow} {audioDevice.FriendlyName} {audioDevice.DeviceFriendlyName} {audioDevice.ID}");

            // Pobierz obecny poziom głośności
            float currentVolume = audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar;

            Console.WriteLine("Obecna głośność: " + currentVolume);

            // Zmień głośność na 50% (można dostosować wartość według własnych potrzeb)
            audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar = 0.1f;
            audioDevice.AudioEndpointVolume.Mute = false;

            Console.WriteLine("Zmieniona głośność: " + audioDevice.AudioEndpointVolume.MasterVolumeLevelScalar);

            PlayYt();
            //var url = "http://media.ch9.ms/ch9/2876/fd36ef30-cfd2-4558-8412-3cf7a0852876/AzureWebJobs103.mp3";
            //using (var mf = new MediaFoundationReader(url))
            //using (var wo = new WasapiOut())
            //{
            //    wo.Init(mf);
            //    wo.Play();
            //    while (wo.PlaybackState == PlaybackState.Playing)
            //    {
            //        Thread.Sleep(1000);
            //    }
            //}


        }

        static async Task PlayYt()
        {
            var youtube = new YoutubeClient();

            var videoUrl = "https://www.youtube.com/watch?v=4WaUZEr7nTE";
            var streamManifest = await youtube.Videos.Streams.GetManifestAsync(videoUrl);

            var streamInfo = streamManifest.GetAudioOnlyStreams().GetWithHighestBitrate();

            var stream = await youtube.Videos.Streams.GetAsync(streamInfo);

            //stream.
            // Download the stream to a file
            //ByteString res = 
            await youtube.Videos.Streams.DownloadAsync(streamInfo, $"video.mp3");



            //var outputPath = "output.mp3"; // Plik wyjściowy w formacie MP3

            //await DownloadAndConvertAudio(videoId, audioStreamInfo, outputPath);

            //// Odtwarzanie pliku MP3 za pomocą NAudio
            using (var audioFile = new AudioFileReader("video.mp3"))
            using (var outputDevice = new WaveOutEvent())
            {
                outputDevice.Init(audioFile);
                outputDevice.Play();
                Console.WriteLine("Naciśnij Enter, aby zakończyć odtwarzanie...");
                Console.ReadLine();
            }

            //// Opcjonalnie: Usuń plik po odtworzeniu
            //File.Delete(outputPath);
        }
    }
}