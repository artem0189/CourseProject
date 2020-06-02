using System;
using System.IO;
using System.Drawing;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CourseProject.ProgramContent.MediaContent;

namespace CourseProject.Model
{
    [Serializable]
    class UserModel : INotifyPropertyChanged
    {
        public UserModel(int id, string name)
        {
            this.id = id;
            this.name = name;
        }

        private int id;
        public int ID
        {
            get
            {
                return id;
            }
            set
            {
                id = value;
                OnPropertyChanged();
            }
        }

        private string name;
        public string Name
        {
            get
            {
                return name;
            }
        }

        private bool isWebcam = false;
        public bool IsWebcam
        { 
            get
            {
                return isWebcam;
            }
            set
            {
                isWebcam = value;
                if (value == false)
                {
                    CurrentFrame = null;
                }
                OnPropertyChanged();
            }
        }

        private bool isMicrophone = false;
        public bool IsMicrophone
        {
            get
            {
                return isMicrophone;
            }
            set
            {
                isMicrophone = value;
                if (VoiceFrame != null)
                {
                    if (value == true)
                    {
                        VoiceFrame.StartOutput();
                    }
                    else
                    {
                        VoiceFrame.StopOutput();
                    }
                }
                OnPropertyChanged();
            }
        }

        private bool isSpeaker = false;
        public bool IsSpeaker
        {
            get
            {
                return isSpeaker;
            }
            set
            {
                isSpeaker = value;
                OnPropertyChanged();
            }
        }

        private bool isScreenDemonstration = false;
        public bool IsScreenDemonstration
        {
            get
            {
                return isScreenDemonstration;
            }
            set
            {
                isScreenDemonstration = value;
                if (value == false && IsWebcam == false)
                {
                    CurrentFrame = null; 
                }
                OnPropertyChanged();
            }
        }

        private Bitmap currentFrame = null;
        public Bitmap CurrentFrame
        {
            get
            {
                return currentFrame;
            }
            set
            {
                currentFrame = value;
                OnPropertyChanged();
            }
        }

        private AudioOutput voiceFrame;
        public AudioOutput VoiceFrame
        {
            get
            {
                return voiceFrame;
            }
            set
            {
                voiceFrame = value;
                OnPropertyChanged();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
