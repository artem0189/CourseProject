using System;
using NAudio.Wave;

namespace CourseProject.ProgramContent.MediaContent
{
    class AudioRecord
    {
        private bool isRunning = false;
        private WaveIn waveIn;
        public AudioRecord(int deviceNumber)
        {
            waveIn = new WaveIn();
            waveIn.DeviceNumber = deviceNumber;
            waveIn.BufferMilliseconds = 10;
            waveIn.WaveFormat = new WaveFormat(16000, 16, 1);
        }

        public event EventHandler<WaveInEventArgs> DataAvailable
        {
            add
            {
                waveIn.DataAvailable += value;
            }
            remove
            {
                waveIn.DataAvailable -= value;
            }
        }

        public void StartRecording()
        {
            waveIn.StartRecording();
            isRunning = true;
        }

        public void StopRecording()
        {
            waveIn.StopRecording();
            isRunning = false;
        }

        public bool IsExist()
        {
            return isRunning;
        }
    }
}
