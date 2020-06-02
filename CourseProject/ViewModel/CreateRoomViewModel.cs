using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using CourseProject.Command;
using CourseProject.ProgramContent;
using CourseProject.ProgramContent.Server;

namespace CourseProject.ViewModel
{
    class CreateRoomViewModel : INotifyPropertyChanged
    {
        private MainViewModel mainVm;

        public CreateRoomViewModel(MainViewModel mainVm)
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

        private RelayCommand createCommand;
        public RelayCommand CreateCommand
        {
            get
            {
                return createCommand ??
                    (new RelayCommand(obj =>
                    {
                        var (result, ip, port) = ParseClass.IPAddressParse(obj.ToString());
                        if (result)
                        {
                            if (ConnectionTesting.TryCreateServer(ip, port))
                            {
                                mainVm.CurrentViewModel = new ChatRoomViewModel(mainVm, ip, port, new Server(ip, port));
                            }
                            else
                            {
                                ErrorMessage = "Не удалось создать комнату";
                            }
                        }
                        else
                        {
                            ErrorMessage = "Некорректный адрес";
                        }
                    },
                    (obj) => true));
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
