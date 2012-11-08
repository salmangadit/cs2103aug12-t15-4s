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
using log4net;

namespace WhiteBoard
{
    //@author U095146E
    /// <summary>
    /// Interaction logic for AutoComplete.xaml
    /// </summary>
    public partial class AutoComplete : UserControl
    {
        #region Private Fields
        private List<String> autoCompleteList;
        private string selectedItem = null;
        #endregion

        #region Protected Fields
        protected static readonly ILog log = LogManager.GetLogger(typeof(AutoComplete));
        #endregion

        #region Constructors
        public AutoComplete()
        {
            InitializeComponent();
            autoCompleteList = new List<string>();
            lstAutoComplete.DataContext = autoCompleteList;
            lstAutoComplete.ItemsSource = autoCompleteList;
        }
        #endregion

        #region Public Properties
        /// <summary>
        /// Returns number of items on AutoComplete List
        /// </summary>
        public int Count
        {
            get
            {
                if (autoCompleteList == null)
                    return 0;
                else
                    return autoCompleteList.Count();
            }
        }

        /// <summary>
        /// Returns currently selected item in AutoComplete List
        /// </summary>
        public string SelectedItem
        {
            get
            {
                if (selectedItem != null)
                    return selectedItem;
                else
                    return null;
            }
        }
        #endregion

        #region Public Event Delegates
        public event KeyEventHandler AutoCompleteKeyboardEvent = delegate { };
        public event MouseEventHandler AutoCompleteMouseEvent = delegate { };
        #endregion

        #region Public Class Methods
        /// <summary>
        /// Display AutoComplete list with results
        /// </summary>
        /// <param name="searchResults">Results retrieved from AutoCompletor</param>
        public void Show(List<string> searchResults)
        {
            log.Debug("Showing Auto Complete List");

            if (searchResults == null)
                throw new NullReferenceException();

            autoCompleteList = searchResults;
            lstAutoComplete.DataContext = autoCompleteList;
            lstAutoComplete.ItemsSource = autoCompleteList;
            lstAutoComplete.Items.Refresh();

            if (autoCompleteList == null)
                return;

            if (autoCompleteList.Count > 0)
            {
                if (autoCompleteList.Count > Constants.AUTOCOMPLETE_MAX_ITEMS)
                {
                    this.Height = Constants.AUTOCOMPLETE_MAX_HEIGHT;
                }
                else
                {
                    this.Height = autoCompleteList.Count * Constants.AUTOCOMPLETE_PER_ITEM_HEIGHT;
                }
            }
        }

        public void Clear()
        {
            autoCompleteList = new List<string>();
            lstAutoComplete.DataContext = autoCompleteList;
            lstAutoComplete.ItemsSource = autoCompleteList;
        }

        #endregion

        #region Private Class Event Handlers
        /// <summary>
        /// Event to handle UserControl getting focus
        /// </summary>
        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lstAutoComplete.Items.Count > 0)
            {
                log.Debug("Focusing on Auto Complete List");
                ListBoxItem item = (ListBoxItem)lstAutoComplete.ItemContainerGenerator.ContainerFromIndex(0);
                FocusManager.SetFocusedElement(lstAutoComplete, item);
            }
        }

        /// <summary>
        /// Event to check if enter key press and trigger appropriate delegate
        /// </summary>
        private void lstAutoComplete_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (lstAutoComplete.SelectedItem == null)
                    return;

                log.Debug("Selecting item from AutoComplete");
                this.selectedItem = lstAutoComplete.SelectedItem.ToString();
                AutoCompleteKeyboardEvent(this, e);
            }
        }

        /// <summary>
        /// Event to check if mouse clicked on item
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void lstAutoComplete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstAutoComplete.SelectedItem == null)
                return;
            log.Debug("Selecting item from AutoComplete");
            this.selectedItem = lstAutoComplete.SelectedItem.ToString();
            AutoCompleteMouseEvent(this, e);
        }
        #endregion
    }
}
