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
        AutoCompletor autoComplete;
        ObservableCollection<Task> tasksOnScreen;
        DispatcherTimer toastTimer;
        DispatcherTimer toastAnimationTimer;
        List<string> keyWords;

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller();
            autoComplete = new AutoCompletor();
            tasksOnScreen = new ObservableCollection<Task>();

            Command command = controller.GetAllTasks(tasksOnScreen.ToList());
            List<Task> tasksToView = command.Execute();
            tasksOnScreen.Clear();
            foreach (Task task in tasksToView)
            {
                tasksOnScreen.Add(task);
            }

            lstTasks.DataContext = tasksOnScreen;
            lstTasks.ItemsSource = tasksOnScreen;
            txtCommand.Focus();

            // Set up syntax highlighting
            keyWords = new List<string>();
            keyWords.Add("MODIFY");
            keyWords.Add("CHANGE");
            keyWords.Add("UPDATE");
            keyWords.Add("SEARCH:");
            keyWords.Add("UNDO:");
            keyWords.Add("DELETE");
            keyWords.Add("REMOVE");
            keyWords.Add("MARK");
            keyWords.Add("VIEW");
        }

        private void DoSyntaxHighlight()
         {
            //@TODO: OOP this by moving the keywords into a static class where the data can be pulled from later

            // Do syntax highlighting
            TextRange userTypedText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
            string userText = userTypedText.Text;

            string[] words = userText.Split(' ');

            if (words.Count() > 0)
            {
                foreach (string keyword in keyWords)
                {
                    if (words[0].ToLower() == keyword.ToLower())
                    {
                        TextRange syntaxHighlight = FindWordFromPosition(txtCommand.Document.ContentStart, words[0]);
                        syntaxHighlight.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Blue));
                        syntaxHighlight.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        userTypedText = new TextRange(syntaxHighlight.End, txtCommand.Document.ContentEnd);
                        break;
                    }
                    else
                    {
                        userTypedText.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                        userTypedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    }
                }

                if (words[0].ToLower() == "view")
                {
                    TextRange syntaxHighlight = FindWordFromPosition(txtCommand.Document.ContentStart, "from");
                    if (syntaxHighlight != null)
                    {
                        syntaxHighlight.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Green));
                        syntaxHighlight.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        userTypedText = new TextRange(syntaxHighlight.End, txtCommand.Document.ContentEnd);
                    }
                    else
                    {
                        userTypedText.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                        userTypedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    }

                    syntaxHighlight = FindWordFromPosition(txtCommand.Document.ContentStart, "to");
                    if (syntaxHighlight != null)
                    {
                        syntaxHighlight.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Green));
                        syntaxHighlight.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        userTypedText = new TextRange(syntaxHighlight.End, txtCommand.Document.ContentEnd);
                    }
                    else
                    {
                        userTypedText.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                        userTypedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    }
                }

                if (words[0].ToLower() == "modify" || words[0].ToLower() == "change" || words[0].ToLower() == "update")
                {
                    TextRange syntaxHighlight = FindWordFromPosition(txtCommand.Document.ContentStart, "start");
                    if (syntaxHighlight != null)
                    {
                        syntaxHighlight.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Green));
                        syntaxHighlight.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        userTypedText = new TextRange(syntaxHighlight.End, txtCommand.Document.ContentEnd);
                    }
                    else
                    {
                        userTypedText.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                        userTypedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    }

                    syntaxHighlight = FindWordFromPosition(txtCommand.Document.ContentStart, "end");
                    if (syntaxHighlight != null)
                    {
                        syntaxHighlight.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Green));
                        syntaxHighlight.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);
                        userTypedText = new TextRange(syntaxHighlight.End, txtCommand.Document.ContentEnd);
                    }
                    else
                    {
                        userTypedText.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                        userTypedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
                    }
                }

                userTypedText.ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(Colors.Black));
                userTypedText.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Normal);
            }
        }

        TextRange FindWordFromPosition(TextPointer position, string word)
        {
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.IndexOf(word);
                    if (indexInRun >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun);
                        TextPointer end = start.GetPositionAtOffset(word.Length);
                        return new TextRange(start, end);
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "word" is not found.
            return null;
        }

        private void txtCommand_KeyUp(object sender, KeyEventArgs e)
        {
            DoSyntaxHighlight();
            // Listen for the press of the enter key
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                TextRange textRange = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);

                string userCommand = textRange.Text;

                if (userCommand == string.Empty)
                    return;

                // Clear search box
                FlowDocument mcFlowDoc = new FlowDocument();
                txtCommand.Document = mcFlowDoc;

                Command command = controller.GetCommandObject(userCommand, tasksOnScreen.ToList());
                if (command == null)
                {
                }
                else if (command.CommandType == CommandType.Add)
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
                else if (command.CommandType == CommandType.View)
                {
                    List<Task> tasksToView = command.Execute();
                    tasksOnScreen.Clear();
                    foreach (Task task in tasksToView)
                    {
                        tasksOnScreen.Add(task);
                    }
                }
                else if (command.CommandType == CommandType.Delete)
                {
                    int deletedTaskId = ((DeleteCommand)command).GetDeletedTaskId();
                    List<Task> deleted = command.Execute();
                    int traversalIndex = 0;
                    List<Task> tasks = tasksOnScreen.ToList<Task>();
                    foreach (Task task in tasks)
                    {
                        if (task.Id == deletedTaskId)
                            tasksOnScreen.RemoveAt(traversalIndex);
                        traversalIndex++;
                    }

                    ShowToast("Deleted task with Id: " + deletedTaskId);
                }
                else if (command.CommandType == CommandType.Archive)
                {
                    int archiveTaskId = ((ArchiveCommand)command).GetArchivedTaskId();
                    List<Task> archived = command.Execute();
                    int traversalIndex = 0;
                    List<Task> tasks = tasksOnScreen.ToList<Task>();
                    foreach (Task task in tasks)
                    {
                        if (task.Id == archiveTaskId)
                            tasksOnScreen.RemoveAt(traversalIndex);
                        traversalIndex++;
                    }

                    ShowToast("Archived task with Id: " + archiveTaskId);
                }
                else if (command.CommandType == CommandType.Search)
                {
                    List<Task> searchResult = command.Execute();
                    string searchString = ((SearchCommand)command).GetSearchString();
                    tasksOnScreen.Clear();
                    foreach (Task task in searchResult)
                    {
                        tasksOnScreen.Add(task);
                    }
                    ShowToast("Search results for: " + searchString);
                }
                else if (command.CommandType == CommandType.Undo)
                {
                    List<Task> previousScreenState = command.Execute();
                    tasksOnScreen.Clear();
                    foreach (Task task in previousScreenState)
                    {
                        tasksOnScreen.Add(task);
                    }
                    ShowToast("Command type undone: " + ((UndoCommand)command).GetUndoCommandType().ToString());
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
            toastTimer.Interval = new TimeSpan(0, 0, 3);
            toastTimer.Start();

            toastAnimationTimer = new DispatcherTimer();
            toastAnimationTimer.Tick += new EventHandler(toastAnimationTimer_Tick);
            toastAnimationTimer.Interval = new TimeSpan(0, 0, 0, 0, 100);
            toastAnimationTimer.Start();
            enter = true;
            toastTime = 0;

            lblToast.Content = toast;
            lblToast.Width = toast.Length * 10;
            lblToast.Visibility = Visibility.Visible;
            lblToast.Opacity = 0;
        }
        bool enter;
        int toastTime;
        private void toastTimer_Tick(object sender, EventArgs e)
        {
            lblToast.Visibility = Visibility.Collapsed;
            toastTimer.Stop();
        }

        private void toastAnimationTimer_Tick(object sender, EventArgs e)
        {
            toastTime++;
            if (lblToast.Opacity < 1 && enter)
            {
                if (lblToast.Opacity >= 0.8)
                {
                    enter = false;
                }
                lblToast.Opacity += 0.2;
            }
            else if (toastTime >= 15)
            {
                if (lblToast.Opacity <= 0.2)
                    toastAnimationTimer.Stop();
                lblToast.Opacity -= 0.2;
            }
        }
    }
}
