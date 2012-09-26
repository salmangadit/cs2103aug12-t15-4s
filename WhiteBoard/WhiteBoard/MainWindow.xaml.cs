using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using System.Windows.Threading;

namespace WhiteBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        Controller controller;
        ObservableCollection<ToDo> tasksList;
        DispatcherTimer toastTimer;

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller();
            tasksList = new ObservableCollection<ToDo>();

            lstTasks.DataContext = tasksList;
            lstTasks.ItemsSource = tasksList;
            //@TODO add method to check file and refresh list with tasks
        }

        private void txtCommand_KeyUp(object sender, KeyEventArgs e)
        {
            // Listen for the press of the enter key
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                string userCommand = txtCommand.Text;

                // Clear search box
                txtCommand.Text = string.Empty;

                Command command = controller.GetCommandObject(userCommand);

                if (command is AddCommand)
                {
                    tasksList.Add(command.Execute());
                }

                ShowToast("Task Added!");

                lstTasks.DataContext = tasksList;
                lstTasks.ItemsSource = tasksList;
                lstTasks.Items.Refresh();
            }
        }

        private void ShowToast(string toast)
        {
            toastTimer = new System.Windows.Threading.DispatcherTimer();
            toastTimer.Tick += new EventHandler(toastTimer_Tick);
            toastTimer.Interval = new TimeSpan(0, 0, 2);
            toastTimer.Start();
            lblToast.Content = toast;
            lblToast.Width = toast.Length * 10;
            lblToast.Visibility = Visibility.Visible;
        }

        private void toastTimer_Tick(object sender, EventArgs e)
        {
            lblToast.Visibility = Visibility.Collapsed;
            toastTimer.Stop();
        }
    }
}
