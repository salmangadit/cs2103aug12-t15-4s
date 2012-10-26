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

namespace WhiteBoard
{
    /// <summary>
    /// Interaction logic for AutoComplete.xaml
    /// </summary>
    public partial class AutoComplete : UserControl
    {
        List<String> autoCompleteList;
        public event KeyEventHandler AutoCompleteKeyboardEvent = delegate { };
        public event MouseEventHandler AutoCompleteMouseEvent = delegate { };

        private string selectedItem = null;

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

        public AutoComplete()
        {
            InitializeComponent();
            autoCompleteList = new List<string>();
            lstAutoComplete.DataContext = autoCompleteList;
            lstAutoComplete.ItemsSource = autoCompleteList;
        }

        public void Show(List<string> searchResults)
        {
            autoCompleteList = searchResults;
            lstAutoComplete.DataContext = autoCompleteList;
            lstAutoComplete.ItemsSource = autoCompleteList;
            lstAutoComplete.Items.Refresh();

            if (autoCompleteList == null)
                return;

            if (autoCompleteList.Count > 0)
            {
                if (autoCompleteList.Count > 10)
                    this.Height = 150;
                else
                    this.Height = autoCompleteList.Count * 18;
            }
        }

        private void UserControl_GotFocus(object sender, RoutedEventArgs e)
        {
            if (lstAutoComplete.Items.Count > 0)
            {
                ListBoxItem item = (ListBoxItem)lstAutoComplete.ItemContainerGenerator.ContainerFromIndex(0);
                FocusManager.SetFocusedElement(lstAutoComplete, item);
            }
        }

        private void lstAutoComplete_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (lstAutoComplete.SelectedItem == null)
                    return;

                this.selectedItem = lstAutoComplete.SelectedItem.ToString();
                AutoCompleteKeyboardEvent(this, e);
            }
        }

        private void lstAutoComplete_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (lstAutoComplete.SelectedItem == null)
                return;

            this.selectedItem = lstAutoComplete.SelectedItem.ToString();
            AutoCompleteMouseEvent(this, e);
        }


    }
}
