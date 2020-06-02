using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CourseProject.Command;
using CourseProject.ProgramContent;
using CourseProject.ProgramContent.Server;

namespace CourseProject.ViewModel
{
    class ConnectRoomViewModel : INotifyPropertyChanged
    {
        private MainViewModel mainVm;

        public ConnectRoomViewModel(MainViewModel mainVm)
        {
            this.mainVm = mainVm;
        }

        private string errorMessage = "";
        public string ErrorMessage
        {
            get
            {
                return errorMessage;
            }
            set
            {
                errorMessage = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand connectCommand;
        public RelayCommand ConnectCommand
        {
            get
            {
                return connectCommand ??
                    (new RelayCommand(obj =>
                    {
                        var (result, ip, port) = ParseClass.IPAddressParse(obj.ToString());
                        if (result)
                        {
                            if (ConnectionTesting.TryConnectToServer(ip, port))
                            {
                                mainVm.CurrentViewModel = new ChatRoomViewModel(mainVm, ip, port, null);
                            }
                            else
                            {
                                ErrorMessage = "Не удалось подключиться";
                            }
                        }
                        else
                        {
                            ErrorMessage = "Некорректный адрес";
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
                        mainVm.CurrentViewModel = new MenuViewModel(mainVm);
                    }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
