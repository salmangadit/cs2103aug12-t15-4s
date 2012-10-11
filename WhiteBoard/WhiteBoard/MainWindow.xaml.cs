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
        ObservableCollection<Task> tasksOnScreen;
        DispatcherTimer toastTimer;

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller();
            tasksOnScreen = new ObservableCollection<Task>();

            lstTasks.DataContext = tasksOnScreen;
            lstTasks.ItemsSource = tasksOnScreen;
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

                Command command = controller.GetCommandObject(userCommand, tasksOnScreen);

                if (command.CommandType == CommandType.Add)
                {
                    Task taskToAdd = (command.Execute())[0];
                    tasksOnScreen.Add(taskToAdd);
                    ShowToast("Task Added!");
                }
                else if (command.CommandType == CommandType.Edit)
                {
                    Task editedTask = (command.Execute())[0];
                    tasksOnScreen.Clear();
                    tasksOnScreen.Add(editedTask);
                    ShowToast("Task with Id " + editedTask.Id + " edited!");
                }

                lstTasks.DataContext = tasksOnScreen;
                lstTasks.ItemsSource = tasksOnScreen;
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
