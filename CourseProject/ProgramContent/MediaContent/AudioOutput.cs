using System;
using NAudio.Wave;

namespace CourseProject.ProgramContent.MediaContent
{
    [Serializable]
    class AudioOutput
    {
        private bool isRunning = false;
        private WaveOut waveOut;
        private BufferedWaveProvider waveProvider;
        public AudioOutput()
        {
            waveProvider = new BufferedWaveProvider(new WaveFormat(16000, 16, 1));
            waveOut = new WaveOut();
            waveOut.DesiredLatency = 100;
            waveOut.Init(waveProvider);
        }

        public void AddData(byte[] data)
        {
            if (isRunning)
            {
                waveProvider.AddSamples(data, 0, data.Length);
            }
        }

        public void StartOutput()
        {
            waveOut.Play();
            isRunning = true;
        }

        public void StopOutput()
        {
            waveOut.Stop();
            isRunning = false;
        }

        public bool IsExist()
        {
            return isRunning;
        }
    }
}
