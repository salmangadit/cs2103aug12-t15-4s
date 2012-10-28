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
        List<string> keywords;
        AutoComplete autoCompleteList;
        Toast toast;

        public MainWindow()
        {
            InitializeComponent();
            controller = new Controller();
            autoComplete = new AutoCompletor();
            autoCompleteList = new AutoComplete();
            autoCompleteList.AutoCompleteKeyboardEvent += new KeyEventHandler(AutoCompleteKeyBubbleEvent);
            autoCompleteList.AutoCompleteMouseEvent += new MouseEventHandler(AutoCompleteMouseBubbleEvent);

            tasksOnScreen = new ObservableCollection<Task>();
            Command command = controller.GetAllTasks(tasksOnScreen.ToList());

            List<Task> tasksToView = command.Execute();
            tasksOnScreen.Clear();
            foreach (Task task in tasksToView)
            {
                tasksOnScreen.Add(task);
            }

            // Data-bind list
            lstTasks.DataContext = tasksOnScreen;
            lstTasks.ItemsSource = tasksOnScreen;
            txtCommand.Focus();

            // Prepare auto complete window
            mainGrid.Children.Add(autoCompleteList);
            Grid.SetRow(autoCompleteList, 0);
            autoCompleteList.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            autoCompleteList.Visibility = Visibility.Collapsed;

            // Set up syntax highlighting
            SyntaxProvider whiteboardSyntax = new SyntaxProvider();
            keywords = whiteboardSyntax.Keywords;

            // Set up toast notification
            toast = new Toast(lblToast);
        }

        #region AutoComplete Delegates
        private void AutoCompleteKeyBubbleEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (autoCompleteList.SelectedItem != null)
                {
                    // first replace the text he has already typed
                    TextRange allText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
                    allText.Text = allText.Text.Replace("\r\n", "");
                    string[] words = allText.Text.Split(' ');
                    if (words.Count() > 1 && !string.IsNullOrWhiteSpace(words[1]))
                    {
                        TextRange replaceText = FindWordFromPosition(txtCommand.Document.ContentStart, words[1]);
                        replaceText = new TextRange(replaceText.Start, txtCommand.Document.ContentEnd);
                        replaceText.Text = "";
                    }

                    // Then append the list box stuff
                    txtCommand.AppendText(autoCompleteList.SelectedItem);
                    autoCompleteList.Visibility = Visibility.Collapsed;
                    txtCommand.Focus();
                    txtCommand.CaretPosition = txtCommand.Document.ContentEnd;
                }
            }
        }

        private void AutoCompleteMouseBubbleEvent(object sender, MouseEventArgs e)
        {
            if (autoCompleteList.SelectedItem != null)
            {
                // first replace the text he has already typed
                TextRange allText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
                allText.Text = allText.Text.Replace("\r\n", "");
                string[] words = allText.Text.Split(' ');
                if (words.Count() > 1 && !string.IsNullOrWhiteSpace(words[1]))
                {
                    TextRange replaceText = FindWordFromPosition(txtCommand.Document.ContentStart, words[1]);
                    replaceText = new TextRange(replaceText.Start, txtCommand.Document.ContentEnd);
                    replaceText.Text = "";
                }

                // Then append the list box stuff
                txtCommand.AppendText(autoCompleteList.SelectedItem);
                autoCompleteList.Visibility = Visibility.Collapsed;
                txtCommand.Focus();
                txtCommand.CaretPosition = txtCommand.Document.ContentEnd;
            }
        }
        #endregion

        #region AutoComplete
        private void CheckAutoComplete()
        {
            // Auto Complete
            TextRange userTextRange = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);

            string command = userTextRange.Text;
            command = command.Replace("\r\n", "");

            string[] words = command.Split(' ');

            if (words.Count() > 0)
            {
                if (words[0].ToLower() == "search:")
                {
                    string search = command.Substring(words[0].Length);
                    autoCompleteList.Show(autoComplete.Query(search));
                }
            }

            if (autoCompleteList.Count == 0)
                autoCompleteList.Visibility = Visibility.Collapsed;
            else
            {
                autoCompleteList.Visibility = Visibility.Visible;
            }

        }
        #endregion

        #region Syntax Highlighting
        private void DoSyntaxHighlight()
        {
            // Do syntax highlighting
            TextRange userTypedText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
            string userText = userTypedText.Text;

            string[] words = userText.Split(' ');

            if (words.Count() > 0)
            {
                foreach (string keyword in keywords)
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
                    TextRange syntaxHighlight = FindLastWordFromPosition(txtCommand.Document.ContentStart, "from");
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

                    syntaxHighlight = FindLastWordFromPosition(txtCommand.Document.ContentStart, "to");
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
                    TextRange syntaxHighlight = FindLastWordFromPosition(txtCommand.Document.ContentStart, "start");
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

                    syntaxHighlight = FindLastWordFromPosition(txtCommand.Document.ContentStart, "end");
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

        TextRange FindLastWordFromPosition(TextPointer position, string word)
        {
            TextRange foundRange = null;
            while (position != null)
            {
                if (position.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    string textRun = position.GetTextInRun(LogicalDirection.Forward);

                    // Find the starting index of any substring that matches "word".
                    int indexInRun = textRun.LastIndexOf(word);
                    if (indexInRun >= 0)
                    {
                        TextPointer start = position.GetPositionAtOffset(indexInRun);
                        if (textRun.Contains(word) && start.GetTextInRun(LogicalDirection.Forward).Trim().Split(' ')[0] == word)
                        {
                            TextPointer end = start.GetPositionAtOffset(word.Length);
                            foundRange = new TextRange(start, end);
                        }
                    }
                }

                position = position.GetNextContextPosition(LogicalDirection.Forward);
            }

            // position will be null if "word" is not found.
            return foundRange;
        }
        #endregion

        private void txtCommand_KeyUp(object sender, KeyEventArgs e)
        {
            CheckAutoComplete();

            TextRange textRange = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
            if (string.IsNullOrWhiteSpace(textRange.Text))
                autoCompleteList.Visibility = System.Windows.Visibility.Collapsed;

            // Listen for the press of the enter key
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                string userCommand = textRange.Text;
                userCommand = userCommand.Replace("\r\n", "");

                if (userCommand == string.Empty)
                    return;

                // Clear search box
                FlowDocument mcFlowDoc = new FlowDocument();
                txtCommand.Document = mcFlowDoc;

                if (autoCompleteList.Visibility == Visibility.Visible)
                    autoCompleteList.Visibility = Visibility.Collapsed;

                Command command = controller.GetCommandObject(userCommand, tasksOnScreen.ToList());
                if (command == null)
                {
                    return;
                }
                else if (command.CommandType == CommandType.Add)
                {
                    ExecuteAdd(command);
                }
                else if (command.CommandType == CommandType.Edit)
                {
                    ExecuteEdit(command);
                }
                else if (command.CommandType == CommandType.View)
                {
                    ExecuteView(command);
                }
                else if (command.CommandType == CommandType.Delete)
                {
                    ExecuteDelete(command);
                }
                else if (command.CommandType == CommandType.Archive)
                {
                    ExecuteArchive(command);
                }
                else if (command.CommandType == CommandType.Search)
                {
                    ExecuteSearch(command);
                }
                else if (command.CommandType == CommandType.Undo)
                {
                    ExecuteUndo(command);
                }

                lstTasks.DataContext = tasksOnScreen;
                lstTasks.ItemsSource = tasksOnScreen;
                lstTasks.Items.Refresh();
            }
            else if ((e.Key == Key.Up || e.Key == Key.Down) && (autoCompleteList.Visibility == Visibility.Visible))
            {
                autoCompleteList.Focus();
            }

            DoSyntaxHighlight();
        }

        private void ExecuteUndo(Command command)
        {
            List<Task> previousScreenState = command.Execute();
            tasksOnScreen.Clear();
            foreach (Task task in previousScreenState)
            {
                tasksOnScreen.Add(task);
            }
            toast.ShowToast("Command type undone: " + ((UndoCommand)command).GetUndoCommandType().ToString());
        }

        private void ExecuteSearch(Command command)
        {
            List<Task> searchResult = command.Execute();
            string searchString = ((SearchCommand)command).GetSearchString();
            tasksOnScreen.Clear();
            foreach (Task task in searchResult)
            {
                tasksOnScreen.Add(task);
            }
            toast.ShowToast("Search results for: " + searchString);
        }

        private void ExecuteArchive(Command command)
        {
            List<int> archiveTaskIds = ((ArchiveCommand)command).GetArchivedTaskIds();
            command.Execute();
            int traversalIndex = 0;
            List<Task> tasks = tasksOnScreen.ToList<Task>();
            foreach (Task task in tasks)
            {
                if (archiveTaskIds.Contains(task.Id))
                    tasksOnScreen.RemoveAt(traversalIndex);
                traversalIndex++;
            }

            StringBuilder tasksArchivedToast = new StringBuilder();

            foreach (int taskId in archiveTaskIds)
            {
                tasksArchivedToast.Append(taskId.ToString() + ", ");
            }

            toast.ShowToast("Archived task(s) with Id: " + tasksArchivedToast);
        }

        private void ExecuteDelete(Command command)
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

            toast.ShowToast("Deleted task with Id: " + deletedTaskId);
        }

        private void ExecuteView(Command command)
        {
            List<Task> tasksToView = command.Execute();
            tasksOnScreen.Clear();
            foreach (Task task in tasksToView)
            {
                tasksOnScreen.Add(task);
            }
        }

        private void ExecuteEdit(Command command)
        {
            Task editedTask = (command.Execute())[0];
            tasksOnScreen.Clear();
            tasksOnScreen.Add(editedTask);
            toast.ShowToast("Task with Id " + editedTask.Id + " edited!");
        }

        private void ExecuteAdd(Command command)
        {
            Task taskToAdd = (command.Execute())[0];
            tasksOnScreen.Add(taskToAdd);
            toast.ShowToast("Task Added!");
        }
    }
}
