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
using System.Diagnostics;
using log4net;
using System.Threading;

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
        List<string> keywords;
        AutoComplete autoCompleteList;
        Toast toast;
        CommandHistory commandHistory;
        protected static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));

        public MainWindow()
        {
            Thread.Sleep(1000);
            InitializeComponent();
            controller = new Controller();
            autoComplete = new AutoCompletor();
            autoCompleteList = new AutoComplete();
            autoCompleteList.AutoCompleteKeyboardEvent += new KeyEventHandler(AutoCompleteKeyBubbleEvent);
            autoCompleteList.AutoCompleteMouseEvent += new MouseEventHandler(AutoCompleteMouseBubbleEvent);

            log4net.Config.XmlConfigurator.Configure();

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

            // Set up command history
            commandHistory = new CommandHistory();
            
            log.Debug("Constructor cleared");
        }

        #region AutoComplete Delegates
        private void AutoCompleteKeyBubbleEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (autoCompleteList.SelectedItem != null)
                {
                    log.Debug("Auto Complete 'Enter' pressed");
                    // first replace the text he has already typed
                    TextRange allText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
                    allText.Text = allText.Text.Replace("\r\n", "");
                    string[] words = allText.Text.Split(' ');
                    
                    Debug.Assert(words.Count() > 0, "No words!");

                    if (words.Count() > 1 && !string.IsNullOrWhiteSpace(words[1]))
                    {
                        log.Debug("Replaced text in command rich text box");
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
                log.Debug("Auto Complete item selected");

                // first replace the text he has already typed
                TextRange allText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
                allText.Text = allText.Text.Replace("\r\n", "");

                string[] words = allText.Text.Split(' ');
                Debug.Assert(words.Count() > 0, "No words!");

                if (words.Count() > 1 && !string.IsNullOrWhiteSpace(words[1]))
                {
                    log.Debug("Replaced text in command rich text box");
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
            log.Debug("Checking AutoComplete requirement");
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

                    try
                    {
                        log.Debug("Generating autocomplete list");
                        autoCompleteList.Show(autoComplete.Query(search));
                    }
                    catch (NullReferenceException)
                    {
                        log.Debug("Search string was null");
                        return;
                    }
                }
            }

            if (autoCompleteList.Count == 0)
            {
                autoCompleteList.Visibility = Visibility.Collapsed;
            }
            else
            {
                autoCompleteList.Visibility = Visibility.Visible;
            }

        }
        #endregion

        #region Syntax Highlighting
        private void DoSyntaxHighlight()
        {
            log.Debug("Checking syntax highlighting");
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
                        log.Debug("Applying syntax highlighting to key word");
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
            Debug.Assert(word != string.Empty, "Looking for empty an string");
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
            Debug.Assert(word != string.Empty, "Looking for empty an string");

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

            // foundRange will be null if "word" is not found.
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
                log.Debug("User clicked enter");

                string userCommand = textRange.Text;
                userCommand = userCommand.Replace("\r\n", "");

                // Return on empty
                if (userCommand == string.Empty)
                    return;

                // Clear search box
                FlowDocument mcFlowDoc = new FlowDocument();
                txtCommand.Document = mcFlowDoc;

                if (autoCompleteList.Visibility == Visibility.Visible)
                    autoCompleteList.Visibility = Visibility.Collapsed;

                // Add to command history
                commandHistory.AddToHistory(userCommand);

                Command command = null;
                try
                {
                    command = controller.GetCommandObject(userCommand, tasksOnScreen.ToList());
                }
                catch (FormatException)
                {
                    toast.ShowToast("Invalid date entered. Please re-enter command");
                }

                if (command == null)
                    return;

                try
                {
                    if (command.CommandType == CommandType.Add)
                    {
                        log.Debug("Adding task");
                        ExecuteAdd(command);
                    }
                    else if (command.CommandType == CommandType.Edit)
                    {
                        log.Debug("Editing task");
                        ExecuteEdit(command);
                    }
                    else if (command.CommandType == CommandType.View)
                    {
                        log.Debug("Viewing task");
                        ExecuteView(command);
                    }
                    else if (command.CommandType == CommandType.Delete)
                    {
                        log.Debug("Deleting task");
                        ExecuteDelete(command);
                    }
                    else if (command.CommandType == CommandType.Archive)
                    {
                        log.Debug("Archiving task");
                        ExecuteArchive(command);
                    }
                    else if (command.CommandType == CommandType.Search)
                    {
                        log.Debug("Searching task");
                        ExecuteSearch(command);
                    }
                    else if (command.CommandType == CommandType.Undo)
                    {
                        log.Debug("Undoing task");
                        ExecuteUndo(command);
                    }
                }
                catch (ApplicationException ex)
                {
                    toast.ShowToast(ex.Message);
                }

                lstTasks.DataContext = tasksOnScreen;
                lstTasks.ItemsSource = tasksOnScreen;
                lstTasks.Items.Refresh();
            }
            else if ((e.Key == Key.Up || e.Key == Key.Down) && (autoCompleteList.Visibility == Visibility.Visible))
            {
                autoCompleteList.Focus();
            }
            else if (e.Key == Key.Up)
            {
                // Clear search box
                FlowDocument mcFlowDoc = new FlowDocument();
                txtCommand.Document = mcFlowDoc;

                txtCommand.AppendText(commandHistory.UpClick());
            }
            else if (e.Key == Key.Down)
            {
                // Clear search box
                FlowDocument mcFlowDoc = new FlowDocument();
                txtCommand.Document = mcFlowDoc;

                txtCommand.AppendText(commandHistory.DownClick());
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
            List<Task> tasks = tasksOnScreen.ToList<Task>();
            
            tasksOnScreen.Clear();

            List<Task> tasksToAdd = tasks.Where(item => !archiveTaskIds.Contains(item.Id)).ToList(); ;

            foreach (Task task in tasksToAdd)
            {
                tasksOnScreen.Add(task);
            }

            StringBuilder tasksArchivedToast = new StringBuilder();

            foreach (int taskId in archiveTaskIds)
            {
                tasksArchivedToast.Append(taskId.ToString() + ", ");
            }

            // remove trailing ,
            tasksArchivedToast.Remove(tasksArchivedToast.Length - 2, 1);

            toast.ShowToast("Archived task(s) with Id: " + tasksArchivedToast);
        }

        private void ExecuteDelete(Command command)
        {
            List<int> deletedTaskIds = ((DeleteCommand)command).GetDeletedTaskId();
            List<Task> deleted = command.Execute();
            List<Task> tasks = tasksOnScreen.ToList<Task>();

            tasksOnScreen.Clear();

            List<Task> tasksToAdd = tasks.Where(item => !deletedTaskIds.Contains(item.Id)).ToList(); ;

            foreach (Task task in tasksToAdd)
            {
                tasksOnScreen.Add(task);
            }

            StringBuilder tasksDeletedToast = new StringBuilder();

            foreach (int taskId in deletedTaskIds)
            {
                tasksDeletedToast.Append(taskId.ToString() + ", ");
            }

            // remove trailing ,
            tasksDeletedToast.Remove(tasksDeletedToast.Length - 2, 1);

            toast.ShowToast("Deleted task with Id: " + tasksDeletedToast);
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
            tasksOnScreen.Clear();
            tasksOnScreen.Add(taskToAdd);
            toast.ShowToast("Task Added!");
        }
    }
}
