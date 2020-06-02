using System.ComponentModel;
using System.Runtime.CompilerServices;
using CourseProject.Command;

namespace CourseProject.ViewModel
{
    class ChooseNameViewModel : INotifyPropertyChanged
    {
        private MainViewModel mainVM;
        public ChooseNameViewModel(MainViewModel mainVM)
        {
            this.mainVM = mainVM;   
        }

        private string name = "";
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value;
                OnPropertyChanged();
            }
        }

        private RelayCommand confirmCommand;
        public RelayCommand ConfirmCommand
        {
            get
            {
                return confirmCommand ??
                    (new RelayCommand(obj =>
                    {
                        mainVM.Name = Name;
                        mainVM.CurrentViewModel = new MenuViewModel(mainVM);
                    },
                    (obj) => name.Length != 0));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName]string prop = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
    }
}
