using System;
using System.IO;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using CourseProject.Model;
using CourseProject.ProgramContent.Server;
using CourseProject.Command;
using AForge.Video.DirectShow;
using System.Drawing;
using NAudio.Wave;
using System.Reflection;
using CourseProject.ProgramContent;
using CourseProject.ProgramContent.MediaContent;

namespace CourseProject.ViewModel
{
    class ChatRoomViewModel : INotifyPropertyChanged, IDisposable
    {
        private MainViewModel mainVM;
        private Server server;
        private int uniqueID;
        private Client client;
        public Webcam webcam;
        private AudioRecord audioRecord;
        private ScreenDemonstration demonstration;

        public ChatRoomViewModel(MainViewModel mainVM, string ip, int remotePort, Server server)
        {
            this.mainVM = mainVM;
            this.server = server;
            uniqueID = (int)(GetHashCode() & 0xFFFFFFFC);

            myModel = new UserModel(uniqueID, mainVM.Name);
            client = new Client(ip, remotePort, uniqueID, this);
            client.SendData(ConvertClass.ObjectToByteArray(myModel), 0, uniqueID + 0);
            users = new List<UserModel>();
            webcam = new Webcam((new FilterInfoCollection(FilterCategory.VideoInputDevice))[0]);
            webcam.NewFrame += (sender, e) =>
            {
                if (myModel.IsScreenDemonstration != true)
                {
                    client.SendData(ConvertClass.ConvertBitmapToByte((Bitmap)e.Frame.Clone()), 0, uniqueID + 1);
                }
            };
            audioRecord = new AudioRecord(0);
            audioRecord.DataAvailable += (sender, e) =>
            {
                client.SendData(e.Buffer, 0, uniqueID + 2);
            };
            demonstration = new ScreenDemonstration();
            demonstration.NewFrame += (sender, e) =>
            {
                client.SendData(ConvertClass.ConvertBitmapToByte((Bitmap)e.Frame.Clone()), 0, uniqueID + 1);
            };
        }

        private List<UserModel> users;
        public List<UserModel> Users
        {
            get
            {
                return users;
            }
            set
            {
                users = value;
                OnPropertyChanged();
            }
        }

        private UserModel myModel;
        public UserModel MyModel
        {
            get
            {
                return myModel;
            }
        }

        private RelayCommand showWebcamCommand;
        public RelayCommand ShowWebcamCommand
        {
            get
            {
                return showWebcamCommand ??
                    (new RelayCommand(obj =>
                    {
                        PropertyInfo property = MyModel.GetType().GetProperty("IsWebcam");
                        property.SetValue(MyModel, !(bool)property.GetValue(MyModel));
                        client.SendData(ConvertClass.ObjectToByteArray(property), 2, uniqueID + 0);
                        if (!webcam.IsExist())
                        {
                            webcam.StartRecording();
                        }
                        else
                        {
                            webcam.StopRecording();
                        }
                    }));
            }
        }

        private RelayCommand audioCommand;
        public RelayCommand AudioCommand
        {
            get
            {
                return audioCommand ??
                    (new RelayCommand(obj =>
                    {
                        PropertyInfo property = MyModel.GetType().GetProperty("IsMicrophone");
                        property.SetValue(MyModel, !(bool)property.GetValue(MyModel));
                        client.SendData(ConvertClass.ObjectToByteArray(property), 2, uniqueID + 0);
                        if (!audioRecord.IsExist())
                        {
                            audioRecord.StartRecording();
                        }
                        else
                        {
                            audioRecord.StopRecording();
                        }
                    }));
            }
        }

        private RelayCommand soundCommand;
        public RelayCommand SoundCommand
        {
            get
            {
                return soundCommand ??
                    (new RelayCommand(obj =>
                    {
                        PropertyInfo property = MyModel.GetType().GetProperty("IsSpeaker");
                        property.SetValue(MyModel, !(bool)property.GetValue(MyModel));
                        client.SendData(ConvertClass.ObjectToByteArray(property), 2, uniqueID + 0);
                    }));
            }
        }

        private RelayCommand demonstrationCommand;
        public RelayCommand DemonstrationCommand
        {
            get
            {
                return demonstrationCommand ??
                    (new RelayCommand(obj =>
                    {
                        PropertyInfo property = MyModel.GetType().GetProperty("IsScreenDemonstration");
                        property.SetValue(MyModel, !(bool)property.GetValue(MyModel));
                        client.SendData(ConvertClass.ObjectToByteArray(property), 2, uniqueID + 0);
                        if (!demonstration.IsExist())
                        {
                            demonstration.StartDemonstration();
                        }
                        else
                        {
                            demonstration.StopDemonstration();
                        }
                    }));
            }
        }

        private RelayCommand exitCommand;
        public RelayCommand ExitCommand
        {
            get
            {
                return exitCommand ??
                    (new RelayCommand(obj =>
                    {
                        Dispose();
                        mainVM.CurrentViewModel = new MenuViewModel(mainVM);
                    }));
            }
        }

        public void DataProcessing(int userID, byte operation, byte[] data)
        {
            UserModel user = Users.Find(obj => obj.ID == (int)(userID & 0xFFFFFFFC));
            switch (userID & 3)
            {
                case 0:
                    switch (operation)
                    {
                        case 0:
                            UserModel newUser = ConvertClass.ByteArrayToObject(data) as UserModel;
                            newUser.VoiceFrame = new AudioOutput();
                            Users = CollectionConversion.ConcatList<UserModel>(users, new List<UserModel>() { newUser });
                            break;
                        case 1:
                            Users = CollectionConversion.ConcatList<UserModel>(users, ConvertClass.ByteArrayToObject(data) as List<UserModel>);
                            foreach (var value in Users)
                            {
                                value.VoiceFrame = new AudioOutput();
                            }
                            break;
                        case 2:
                            PropertyInfo property = ConvertClass.ByteArrayToObject(data) as PropertyInfo;
                            property.SetValue(user, !(bool)property.GetValue(user));
                            break;
                        case 3:
                            mainVM.CurrentViewModel = new MenuViewModel(mainVM);
                            break;
                        case 4:
                            Users = (ConvertClass.ByteArrayToObject(data) as List<UserModel>).FindAll(obj => obj.ID != uniqueID);
                            break;
                    }
                    break;
                case 1:
                    if (user.IsWebcam != false || user.IsScreenDemonstration != false)
                    {
                        user.CurrentFrame = new Bitmap(new MemoryStream(data));
                    }
                    break;
                case 2:
                    if (MyModel.IsSpeaker)
                    {
                        user.VoiceFrame.AddData(data);
                    }
                    break;
            }
        }

        public void Dispose()
        {
            webcam.StopRecording();
            audioRecord.StopRecording();
            demonstration.StopDemonstration();
            if (client != null)
            {
                client.Dispose();
            }
            if (server != null)
            {
                server.Dispose();
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
