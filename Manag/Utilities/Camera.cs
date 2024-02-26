using AForge.Video;
using AForge.Video.DirectShow;
using AForge.Video.FFMPEG;
using Grpc.Core;
using System.Drawing;

namespace Manag.Utilities
{
    class Camera
    {
        //private static FilterInfoCollection videoDevices;
        private VideoCaptureDevice _videoSource;
        private bool _takedPhoto = false;
        private Bitmap? _photo;
        private List<Bitmap> _film;
        private VideoFileWriter _writer;

        internal Camera()
        {
            var videoDevices = new FilterInfoCollection(FilterCategory.VideoInputDevice);

            if (videoDevices.Count == 0)
            {
                throw new RpcException(new Status(StatusCode.Internal, "not found any camera"));
            }

            _videoSource = new VideoCaptureDevice(videoDevices[0].MonikerString);

            var highestResolution = _videoSource.VideoCapabilities.OrderByDescending(c => c.FrameSize.Width * c.FrameSize.Height).First();
            _videoSource.VideoResolution = highestResolution;
        }
        internal Bitmap? TakePhoto()
        {
            _takedPhoto = false;

            var photoEventHandler = new NewFrameEventHandler(photoEvent);
            _videoSource.NewFrame += photoEventHandler;

            _videoSource.Start();


            while (!_takedPhoto) { }

            _videoSource.SignalToStop();
            _videoSource.WaitForStop();

            _videoSource.NewFrame -= photoEventHandler;

            return _photo;
        }

        internal void MakeFilm()
        {
            var filmEventHandler = new NewFrameEventHandler(filmEvent);
            _videoSource.NewFrame += filmEventHandler;

            _writer = new VideoFileWriter();
            _writer.Open("bobas.avi", 640, 480, 25, VideoCodec.MPEG4);

            _videoSource.Start();

            Thread.Sleep(4000);

            _videoSource.SignalToStop();
            _videoSource.WaitForStop();
            _writer.Close();
            //string videoPath = "output.avi";

            //VideoFileWriter videoWriter = new VideoFileWriter();



            _videoSource.NewFrame -= filmEventHandler;

            //return _photo;


            //VideoFileWriter writer = new VideoFileWriter();
            //_takedPhoto = false;

            //var photoEventHandler = new NewFrameEventHandler(photoEvent);
            //_videoSource.NewFrame += photoEventHandler;

            //_videoSource.Start();


            //while (!_takedPhoto) { }

            //_videoSource.SignalToStop();
            //_videoSource.WaitForStop();

            //return _photo;
        }


        private void photoEvent(object sender, NewFrameEventArgs eventArgs)
        {
            _photo = (Bitmap)eventArgs.Frame.Clone();
            _takedPhoto = true;
        }

        private void filmEvent(object sender, NewFrameEventArgs eventArgs)
        {
            var bitmap = (Bitmap)eventArgs.Frame.Clone();
            //_film.Add((Bitmap)eventArgs.Frame.Clone());
            _writer.WriteVideoFrame(bitmap);
        }

    }
}
