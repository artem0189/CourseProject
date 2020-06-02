using AForge.Video;
using AForge.Video.DirectShow;

namespace CourseProject.ProgramContent.MediaContent
{
    class Webcam
    {
        private VideoCaptureDevice videoSource;
        public Webcam(FilterInfo videoDevice)
        {
            videoSource = new VideoCaptureDevice(videoDevice.MonikerString);
        }

        public event NewFrameEventHandler NewFrame
        {
            add
            {
                videoSource.NewFrame += value;
            }
            remove
            {
                videoSource.NewFrame -= value;
            }
        }

        public void StartRecording()
        {
            videoSource.Start();
        }

        public void StopRecording()
        {
            videoSource.Stop();
        }

        public bool IsExist()
        {
            return videoSource.IsRunning;
        }
    }
}
