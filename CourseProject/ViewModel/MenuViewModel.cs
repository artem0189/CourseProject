using System.ComponentModel;
using System.Runtime.CompilerServices;
using CourseProject.Command;

namespace CourseProject.ViewModel
{
    class MenuViewModel : INotifyPropertyChanged
    {
        private MainViewModel mainVm;
        public MenuViewModel(MainViewModel mainVm)
        {
            this.mainVm = mainVm;
        }

        private RelayCommand newRoomCommand;
        public RelayCommand NewRoomCommand
        {
            get
            {
                return newRoomCommand ??
                    (new RelayCommand(obj =>
                    {
                        mainVm.CurrentViewModel = new CreateRoomViewModel(mainVm);
                    }));
            }
        }

        private RelayCommand enterRoomCommand;
        public RelayCommand EnterRoomCommand
        {
            get
            {
                return enterRoomCommand ??
                    (new RelayCommand(obj =>
                    {
                        mainVm.CurrentViewModel = new ConnectRoomViewModel(mainVm);
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
                        System.Windows.Application.Current.Shutdown();
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
