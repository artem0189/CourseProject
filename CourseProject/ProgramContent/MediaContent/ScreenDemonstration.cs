using System;
using System.Windows;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Drawing;
using System.Drawing.Imaging;
using CourseProject.ProgramContent;
using AForge.Video;
using AForge.Video.DirectShow;

namespace CourseProject.ProgramContent.MediaContent
{
    class ScreenDemonstration
    {
        private TimerCallback tm;
        private Timer timer;
        private bool isRunning = false; 
        public ScreenDemonstration()
        {
            tm = new TimerCallback(GetCurrentFrame);
            timer = new Timer(new TimerCallback(GetCurrentFrame), null, Timeout.Infinite, Timeout.Infinite);
        }

        private event NewFrameEventHandler newFrame;
        public event NewFrameEventHandler NewFrame
        {
            add
            {
                newFrame += value;
            }
            remove
            {
                newFrame -= value;
            }
        }

        private void GetCurrentFrame(object obj)
        {
            Bitmap bitmap;
            bitmap = new Bitmap((int)SystemParameters.PrimaryScreenWidth, (int)SystemParameters.PrimaryScreenHeight, PixelFormat.Format32bppArgb);
            using (Graphics g = Graphics.FromImage(bitmap))
            {
                g.CopyFromScreen(0, 0, 0, 0, bitmap.Size);
            }
            newFrame?.Invoke(this, new NewFrameEventArgs(bitmap));
            bitmap.Dispose();
        }

        public void StartDemonstration()
        {
            timer.Change(0, 100);
            isRunning = true;
        }

        public void StopDemonstration()
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            isRunning = false;
        }

        public bool IsExist()
        {
            return isRunning;
        }
    }
}
