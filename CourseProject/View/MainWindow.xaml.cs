using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using CourseProject.ViewModel;

namespace CourseProject.View
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            this.Closed += MainWindow_Closed;
            this.Closing += MainWindow_Closing;
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if ((this.DataContext as MainViewModel).CurrentViewModel is ChatRoomViewModel)
            {
                ((this.DataContext as MainViewModel).CurrentViewModel as ChatRoomViewModel).webcam.StopRecording();
                ((this.DataContext as MainViewModel).CurrentViewModel as ChatRoomViewModel).Dispose();
            }
        }

        private void MainWindow_Closed(object sender, EventArgs e)
        {
            if ((this.DataContext as MainViewModel).CurrentViewModel is ChatRoomViewModel)
            {
                ((this.DataContext as MainViewModel).CurrentViewModel as ChatRoomViewModel).webcam.StopRecording();
                ((this.DataContext as MainViewModel).CurrentViewModel as ChatRoomViewModel).Dispose();
            }
        }
    }
}
