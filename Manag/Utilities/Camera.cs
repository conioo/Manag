using AForge.Video;
using AForge.Video.DirectShow;
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

            return _photo;
        }
        private void photoEvent(object sender, NewFrameEventArgs eventArgs)
        {
            _photo = (Bitmap)eventArgs.Frame.Clone();
            _takedPhoto = true;
        }
    }
}
