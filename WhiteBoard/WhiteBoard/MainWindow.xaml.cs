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
using System.IO;

namespace WhiteBoard
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    //@author U095146E
    public partial class MainWindow : Window
    {
        #region Private Fields
        private Controller controller;
        private AutoCompletor autoComplete;
        private ObservableCollection<Task> tasksOnScreen;
        private List<string> keywords;
        private AutoComplete autoCompleteList;
        private Toast toast;
        private CommandHistory commandHistory;
        private SyntaxProvider whiteboardSyntax;
        private Tutorial whiteboardTutorial = null;

        bool showTutorial = false;
        #endregion

        #region Protected Fields
        protected static readonly ILog log = LogManager.GetLogger(typeof(MainWindow));
        #endregion

        #region Constructors
        public MainWindow()
        {
            Thread.Sleep(1000); //For Splash Screen

            InitializeComponent();
            CheckIfFileExists();
            InstantiatePrivateComponents();
            DisplayTasksOnFirstLoad();
            InstantiateUserControls();

            keywords = whiteboardSyntax.Keywords;

            log.Debug("Main Constructor end");
        }
        #endregion

        #region Private Delegate Handlers
        #region AutoComplete Delegates

        /// <summary>
        /// Delegate to handle enter key-press in AutoComplete User Control
        /// </summary>
        private void AutoCompleteKeyBubbleEvent(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (autoCompleteList.SelectedItem != null)
                {
                    log.Debug("Auto Complete 'Enter' pressed");

                    // First replace the text the user has already typed
                    TextRange allText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
                    allText.Text = allText.Text.Replace("\r\n", "");
                    string[] words = allText.Text.Split(' ');

                    Debug.Assert(words.Count() > 0, "No words!");

                    if (words.Count() > 1 && !string.IsNullOrWhiteSpace(words[1]))
                    {
                        log.Debug("Replaced text in command rich text box");

                        TextRange replaceText = FindLastWordFromPosition(txtCommand.Document.ContentStart, words[1]);
                        replaceText = new TextRange(replaceText.Start, txtCommand.Document.ContentEnd);
                        replaceText.Text = "";
                    }

                    // Then append the command textbox with the AutoComplete Item
                    txtCommand.AppendText(autoCompleteList.SelectedItem);
                    autoCompleteList.Visibility = Visibility.Collapsed;
                    txtCommand.Focus();
                    txtCommand.CaretPosition = txtCommand.Document.ContentEnd;

                    // Then simulate search event trigger
                    var key = Key.Enter;
                    var target = txtCommand;
                    var routedEvent = Keyboard.KeyUpEvent;

                    target.RaiseEvent(
                      new KeyEventArgs(
                        Keyboard.PrimaryDevice,
                        PresentationSource.FromVisual(target),
                        0,
                        key) { RoutedEvent = routedEvent }
                    );
                }
            }
        }

        /// <summary>
        /// Delegate to handle enter mouse click in AutoComplete User Control
        /// </summary>
        private void AutoCompleteMouseBubbleEvent(object sender, MouseEventArgs e)
        {
            if (autoCompleteList.SelectedItem != null)
            {
                log.Debug("Auto Complete item selected");

                // first replace the text the user has already typed
                TextRange allText = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
                allText.Text = allText.Text.Replace("\r\n", "");

                string[] words = allText.Text.Split(' ');
                Debug.Assert(words.Count() > 0, "No words!");

                if (words.Count() > 1 && !string.IsNullOrWhiteSpace(words[1]))
                {
                    log.Debug("Replaced text in command rich text box");
                    TextRange replaceText = FindLastWordFromPosition(txtCommand.Document.ContentStart, words[1]);
                    replaceText = new TextRange(replaceText.Start, txtCommand.Document.ContentEnd);
                    replaceText.Text = "";
                }

                // Then append the command textbox with the AutoComplete Item
                txtCommand.AppendText(autoCompleteList.SelectedItem);
                autoCompleteList.Visibility = Visibility.Collapsed;
                txtCommand.Focus();
                txtCommand.CaretPosition = txtCommand.Document.ContentEnd;

                // Then simulate search  event trigger
                var key = Key.Enter;
                var target = txtCommand;
                var routedEvent = Keyboard.KeyUpEvent;

                target.RaiseEvent(
                  new KeyEventArgs(
                    Keyboard.PrimaryDevice,
                    PresentationSource.FromVisual(target),
                    0,
                    key) { RoutedEvent = routedEvent }
                );
            }
        }
        #endregion
        #endregion

        #region Private Class Event Handlers
        /// <summary>
        /// Event to handle each keypress on Command textbox
        /// </summary>
        private void txtCommand_KeyUp(object sender, KeyEventArgs e)
        {
            CheckAutoComplete();

            // Get all text in command box
            TextRange textRange = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);

            if (string.IsNullOrWhiteSpace(textRange.Text))
                autoCompleteList.Visibility = System.Windows.Visibility.Collapsed;

            // Listen for the press of the enter key
            if (e.Key == Key.Enter || e.Key == Key.Return)
            {
                log.Debug("User clicked enter");
                EnterKeyPressed(textRange);
            }
            else if ((e.Key == Key.Up || e.Key == Key.Down) && (autoCompleteList.Visibility == Visibility.Visible))
            {
                autoCompleteList.Focus();
            }
            else if (e.Key == Key.Up)
            {
                UpKeyPressed();
            }
            else if (e.Key == Key.Down)
            {
                DownKeyPressed();
            }
            else if ((Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl)) && e.Key == Key.Z)
            {
                log.Debug("Undoing task");
                ControlZPressed();
            }

            DoSyntaxHighlight();
        }


        private void btnHelp_Click(object sender, RoutedEventArgs e)
        {
            whiteboardTutorial.Visibility = Visibility.Visible;
        }
        #endregion

        #region Private Class Helper Methods
        /// <summary>
        /// Instantiate all components used
        /// </summary>
        private void InstantiatePrivateComponents()
        {
            controller = new Controller();
            autoComplete = new AutoCompletor();
            autoCompleteList = new AutoComplete();
            autoCompleteList.AutoCompleteKeyboardEvent += new KeyEventHandler(AutoCompleteKeyBubbleEvent);
            autoCompleteList.AutoCompleteMouseEvent += new MouseEventHandler(AutoCompleteMouseBubbleEvent);
            tasksOnScreen = new ObservableCollection<Task>();
            toast = new Toast(lblToast);
            commandHistory = new CommandHistory();
            whiteboardSyntax = new SyntaxProvider();
            if (whiteboardTutorial == null)
                whiteboardTutorial = new Tutorial();

            log4net.Config.XmlConfigurator.Configure();
        }

        /// <summary>
        /// Display all tasks when loaded for the first time
        /// </summary>
        private void DisplayTasksOnFirstLoad()
        {
            Command command = controller.GetAllTasks(tasksOnScreen.ToList());
            try
            {
                List<Task> tasksToView = command.Execute();

                tasksOnScreen.Clear();
                foreach (Task task in tasksToView)
                {
                    tasksOnScreen.Add(task);
                }
            }
            catch (ApplicationException ex)
            {
                if (!showTutorial)
                {
                    toast.ShowToast(ex.Message, this);
                }
            }
            // Data-bind list
            lstTasks.DataContext = tasksOnScreen;
            lstTasks.ItemsSource = tasksOnScreen;
            txtCommand.Focus();
        }

        /// <summary>
        /// Checking existence of file before deciding to show tutorial
        /// </summary>
        private void CheckIfFileExists()
        {
            // Check if File Exists
            string fileName = Constants.FILENAME;

            // set file path, we use the current Directory for the user and specified file name
            string filePath = Directory.GetCurrentDirectory() + System.IO.Path.DirectorySeparatorChar + fileName;

            // check if file exists and create one if need be
            if (!File.Exists(filePath))
            {
                // Show tutorial
                showTutorial = true;
            }
        }

        /// <summary>
        /// Instantiate all user controls required in this application
        /// </summary>
        private void InstantiateUserControls()
        {
            // Prepare auto complete window
            mainGrid.Children.Add(autoCompleteList);
            Grid.SetRow(autoCompleteList, 0);
            autoCompleteList.VerticalAlignment = System.Windows.VerticalAlignment.Bottom;
            autoCompleteList.Visibility = Visibility.Collapsed;

            // Prepare tutorial window
            mainGrid.Children.Add(whiteboardTutorial);
            Grid.SetRow(whiteboardTutorial, 0);

            if (showTutorial)
            {
                whiteboardTutorial.Visibility = Visibility.Visible;
            }
            else
            {
                whiteboardTutorial.Visibility = Visibility.Collapsed;
            }
        }

        #region AutoComplete
        /// <summary>
        /// Search AutoComplete items and populate user control
        /// </summary>
        private void CheckAutoComplete()
        {
            log.Debug("Checking AutoComplete requirement");

            TextRange userTextRange = new TextRange(txtCommand.Document.ContentStart, txtCommand.Document.ContentEnd);
            InstantSearch instantSearch = new InstantSearch();
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
                        DisplayInstantSearch(instantSearch.GetTasksWithDescription(search));
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

        /// <summary>
        /// Display search results from instant search on screen
        /// </summary>
        /// <param name="results"></param>
        private void DisplayInstantSearch(List<Task> results)
        {
            if (results.Count == 0)
                return;

            tasksOnScreen.Clear();
            foreach (Task task in results)
            {
                tasksOnScreen.Add(task);
            }

            lstTasks.DataContext = tasksOnScreen;
            lstTasks.ItemsSource = tasksOnScreen;
            lstTasks.Items.Refresh();
        }
        #endregion

        #region Syntax Highlighting
        /// <summary>
        /// Carries out syntax highlighting on typed text
        /// </summary>
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

                if (words[0].ToLower() == "modify" || words[0].ToLower() == "update")
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

        /// <summary>
        /// Finds a specified word and returns the appropriate text range
        /// </summary>
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

        /// <summary>
        /// Finds last instance of a word and returns appropriate text range
        /// </summary>
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

        /// <summary>
        /// Execution when 'enter' key is pressed
        /// </summary>
        /// <param name="textRange"></param>
        private void EnterKeyPressed(TextRange textRange)
        {
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

            autoCompleteList.Clear();

            // Add to command history
            commandHistory.AddToHistory(userCommand);

            Command command = null;
            try
            {
                command = controller.GetCommandObject(userCommand, tasksOnScreen.ToList());
            }
            catch (FormatException)
            {
                toast.ShowToast(Constants.INVALID_DATE_FORMAT, this);
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
                else if (command.CommandType == CommandType.UnArchive)
                {
                    log.Debug("Unarchiving task");

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
                toast.ShowToast(ex.Message, this);
            }

            lstTasks.DataContext = tasksOnScreen;
            lstTasks.ItemsSource = tasksOnScreen;
            lstTasks.Items.Refresh();
        }

        /// <summary>
        /// Executes undo for CTRL+Z
        /// </summary>
        private void ControlZPressed()
        {
            Command command = controller.Undo(tasksOnScreen.ToList());

            if (command == null)
                return;

            try
            {
                ExecuteUndo(command);
            }
            catch (ApplicationException ex)
            {
                toast.ShowToast(ex.Message, this);
            }
            // Data-bind list
            lstTasks.DataContext = tasksOnScreen;
            lstTasks.ItemsSource = tasksOnScreen;
            lstTasks.Items.Refresh();
            return;
        }

        /// <summary>
        /// Show next item in command history
        /// </summary>
        private void DownKeyPressed()
        {
            // Clear search box
            FlowDocument mcFlowDoc = new FlowDocument();
            txtCommand.Document = mcFlowDoc;

            txtCommand.AppendText(commandHistory.DownClick());
        }

        /// <summary>
        /// Show previous item in command history
        /// </summary>
        private void UpKeyPressed()
        {
            // Clear search box
            FlowDocument mcFlowDoc = new FlowDocument();
            txtCommand.Document = mcFlowDoc;

            txtCommand.AppendText(commandHistory.UpClick());
        }

        /// <summary>
        /// Perform undo task
        /// </summary>
        private void ExecuteUndo(Command command)
        {
            List<Task> previousScreenState = command.Execute();
            tasksOnScreen.Clear();
            foreach (Task task in previousScreenState)
            {
                tasksOnScreen.Add(task);
            }
            toast.ShowToast(Constants.MESSAGE_COMMAND_UNDO + ((UndoCommand)command).GetUndoCommandType().ToString(), this);
        }

        /// <summary>
        /// Perform search operation
        /// </summary>
        private void ExecuteSearch(Command command)
        {
            List<Task> searchResult = command.Execute();
            string searchString = ((SearchCommand)command).GetSearchString();
            tasksOnScreen.Clear();
            foreach (Task task in searchResult)
            {
                tasksOnScreen.Add(task);
            }
            toast.ShowToast(Constants.MESSAGE_COMMAND_SEARCH + searchString, this);
        }

        /// <summary>
        /// Perform archive operation
        /// </summary>
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

            toast.ShowToast(Constants.MESSAGE_COMMAND_ARCHIVE + tasksArchivedToast, this);
        }

        /// <summary>
        /// Perform unarchive operation
        /// </summary>
        private void ExecuteUnarchive(Command command)
        {
            List<int> unArchiveTaskIds = ((UnArchiveCommand)command).GetUnArchivedTaskIds();
            command.Execute();
            List<Task> tasks = tasksOnScreen.ToList<Task>();

            tasksOnScreen.Clear();

            List<Task> tasksToAdd = tasks.Where(item => !unArchiveTaskIds.Contains(item.Id)).ToList(); ;

            foreach (Task task in tasksToAdd)
            {
                tasksOnScreen.Add(task);
            }

            StringBuilder tasksUnarchivedToast = new StringBuilder();

            foreach (int taskId in unArchiveTaskIds)
            {
                tasksUnarchivedToast.Append(taskId.ToString() + ", ");
            }

            // remove trailing ,
            tasksUnarchivedToast.Remove(tasksUnarchivedToast.Length - 2, 1);

            toast.ShowToast(Constants.MESSAGE_COMMAND_ARCHIVE + tasksUnarchivedToast, this);
        }

        /// <summary>
        /// Perform delete operation
        /// </summary>
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

            toast.ShowToast(Constants.MESSAGE_COMMAND_DELETE + tasksDeletedToast, this);
        }

        /// <summary>
        /// Perform view operation
        /// </summary>
        private void ExecuteView(Command command)
        {
            List<Task> tasksToView = command.Execute();
            tasksOnScreen.Clear();
            foreach (Task task in tasksToView)
            {
                tasksOnScreen.Add(task);
            }
        }

        /// <summary>
        /// Perform Edit Operation
        /// </summary>
        private void ExecuteEdit(Command command)
        {
            Task editedTask = (command.Execute())[0];
            tasksOnScreen.Clear();
            tasksOnScreen.Add(editedTask);
            toast.ShowToast(Constants.MESSAGE_COMMAND_EDIT + editedTask.Id, this);
        }

        /// <summary>
        /// Perform Add Operation
        /// </summary>
        private void ExecuteAdd(Command command)
        {
            Task taskToAdd = (command.Execute())[0];
            tasksOnScreen.Clear();
            tasksOnScreen.Add(taskToAdd);
            toast.ShowToast(Constants.MESSAGE_COMMAND_ADD, this);
        }
        #endregion

    }
}
