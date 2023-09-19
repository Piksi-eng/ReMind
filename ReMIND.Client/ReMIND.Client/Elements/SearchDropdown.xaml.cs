using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for SearchBox.xaml
    /// </summary>
    public partial class SearchDropdown : UserControl
    {

        /// <summary>
        /// Sends the string of the selected item every time the dropdown selection changed (including deselection)
        /// </summary>
        public event EventHandler<object> SelectedItemChanged;
        public List<object> SearchList;

        public SearchDropdown()
        {
            InitializeComponent();
            SearchList = new();
            DataContext = this;
            loadTestData();
        }

        public bool ResetButtonVisible
        {
            get => resetButton.Visibility == Visibility.Visible;
            set => resetButton.Visibility = value ?
                Visibility.Visible : Visibility.Collapsed;
        }
        public string Placeholder { get; set; }


        public string SelectedItem => getSelectedString();

        private void Control_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter || e.Key == Key.Tab)
            {
                attemptSelect();
                unFocus();
            }
            else if (e.Key == Key.Escape)
            {
                InputText.Text = "";
                SelectedItemChanged?.Invoke(this, "");
                unFocus();
            }
        }


        private void InputText_TextChanged(object sender, TextChangedEventArgs e)
        {
            if(InputText.Text != "")
            {
                PlaceholderText.Visibility = Visibility.Collapsed;
                if(ListScale.ScaleY == 0)
                    DropdownButton.IsChecked = true;
            }
            foreach (DropdownItem di in ItemsContainer.Children)
            {
                di.Visibility = di.AttachedString.Contains(InputText.Text, StringComparison.CurrentCultureIgnoreCase) ?
                    Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void ClearSelectionWithoutEvents()
        {
            InputText.Text = "";
            PlaceholderText.Visibility = Visibility.Visible;
            unFocus();
        }

        #region Focus, unFocus...

        /// <summary>
        /// Serves as the "focus" method, focusing the text input field when user clicks on the control
        /// Work in conjunction with <see cref="Border_PreviewMouseDown(object, MouseButtonEventArgs)"/>
        /// </summary>
        private void Border_GotFocus(object sender, EventArgs e)
        {
            PlaceholderText.Visibility = Visibility.Collapsed;
            InputText.Focus();
            InputText.SelectAll();
        }

        /// <summary>
        /// Look at <see cref="Border_GotFocus(object, EventArgs)"/>
        /// </summary>
        private void Border_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            PlaceholderText.Visibility = Visibility.Collapsed;
            InputText.Focus();
            InputText.SelectAll();
        }

        private void InputText_LostFocus(object sender, RoutedEventArgs e)
        {
            if(getSelectedString() == "")
            {
                PlaceholderText.Visibility = Visibility.Visible;
                DropdownButton.IsChecked = false;
            }
        }

        /// <summary>
        /// Ensures no random string is left in the field when it loses focus
        /// </summary>
        private void UserControl_LostFocus(object sender, EventArgs e)
        {
            if (getSelectedString() == "")
            {
                PlaceholderText.Visibility = Visibility.Visible;
                InputText.Text = "";
            }
            DropdownButton.IsChecked = false;
        }


        private void unFocus()
        {
            FrameworkElement parent = (FrameworkElement)Parent;
            while (parent != null && parent is IInputElement element && !element.Focusable)
            {
                parent = (FrameworkElement)parent.Parent;
            }

            DependencyObject scope = FocusManager.GetFocusScope(this);
            FocusManager.SetFocusedElement(scope, parent);
        }
        #endregion

        #region Dropdown UI
        private void DropdownButton_Checked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new(1, TimeSpan.FromMilliseconds(150));
            ListScale.BeginAnimation(ScaleTransform.ScaleYProperty, animation);

            DoubleAnimation animation2 = new(0, 180, TimeSpan.FromMilliseconds(150));
            ButtonRotate.BeginAnimation(RotateTransform.AngleProperty, animation2);

            if(getSelectedString() != "")
                foreach (UIElement child in ItemsContainer.Children)
                    child.Visibility = Visibility.Visible;
        }

        private void DropdownButton_Unchecked(object sender, RoutedEventArgs e)
        {
            DoubleAnimation animation = new(0, TimeSpan.FromMilliseconds(150));
            ListScale.BeginAnimation(ScaleTransform.ScaleYProperty, animation);

            DoubleAnimation animation2 = new(180, 0, TimeSpan.FromMilliseconds(150));
            ButtonRotate.BeginAnimation(RotateTransform.AngleProperty, animation);
        }

        private void Placeholder_Click(object sender, RoutedEventArgs e)
        {
            InputText.Text = "";
            DropdownButton.IsChecked = false;
            SelectedItemChanged?.Invoke(this, "");
            unFocus();
        }


        #endregion

        #region List data methods

        public object getSelectedObject()
        {//vraca selected object bez menjanja bilo cega
            foreach (DropdownItem child in ItemsContainer.Children)
            {
                if (child.AttachedString.Equals(InputText.Text, StringComparison.CurrentCultureIgnoreCase))
                    return child.AttachedObject;
            }
            return null;
        }
        public string getSelectedString()
        {//vraca selected string bez menjanja bilo cega
            foreach(DropdownItem child in ItemsContainer.Children)
            {
                if (child.AttachedString.Equals(InputText.Text, StringComparison.CurrentCultureIgnoreCase))
                    return child.AttachedString;
            }
            return "";
        }
        public bool attemptSelect()
        {//attempts to select closest item, otherwise selects ""
            foreach(DropdownItem child in ItemsContainer.Children)
            {
                if (child.AttachedString.Contains(InputText.Text, StringComparison.CurrentCultureIgnoreCase))
                {
                    InputText.Text = child.AttachedString;
                    SelectedItemChanged?.Invoke(this, child.AttachedObject);
                    return true;
                }
            }
            InputText.Text = "";
            SelectedItemChanged?.Invoke(this, "");
            return false;
        }
        public void selectFirst()
        {//selects the closest item to the search query, if it doesn't exist, selects the first in the list
            if (!attemptSelect())
            {
                var tb = (DropdownItem)ItemsContainer.Children[0];
                InputText.Text = tb.AttachedString;
            }
        }
        public void selectObject(object o)
        {//attempts to select the object sent as an argument, if it doesn't exist, nothing
            foreach(DropdownItem di in ItemsContainer.Children)
                if (di.AttachedObject.Equals(o))
                {
                    InputText.Text = o.ToString();
                    SelectedItemChanged?.Invoke(this, di.AttachedObject);
                }
            DropdownButton.IsChecked = false;
        }
        public void selectObjectWithoutEvents(object o)
        {//same as the one above, but without events
            foreach (DropdownItem di in ItemsContainer.Children)
                if (di.AttachedObject.Equals(o))
                    InputText.Text = o.ToString();
            DropdownButton.IsChecked = false;
        }

        public void clear()
        {
            if(InputText.Text != "")
                SelectedItemChanged?.Invoke(this, "");

            InputText.Text = "";
            DropdownButton.IsChecked = false;
            PlaceholderText.Visibility = Visibility.Visible;
        }
        #endregion

        #region Mouse Item Actions
        private void TextBlock_MouseEnter(object sender, MouseEventArgs e)
        {
            DropdownItem di = (DropdownItem)sender;
            di.Background = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
            di.Foreground = new BrushConverter().ConvertFromString("#266986") as SolidColorBrush;
        }

        private void TextBlock_MouseLeave(object sender, MouseEventArgs e)
        {
            DropdownItem di = (DropdownItem)sender;
            di.Background = new BrushConverter().ConvertFromString("#266986") as SolidColorBrush;
            di.Foreground = new BrushConverter().ConvertFromString("#FFFFFF") as SolidColorBrush;
        }

        private void TextBlock_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            DropdownItem di = (DropdownItem)sender;
            InputText.Text = di.AttachedString;
            DropdownButton.IsChecked = false;
            SelectedItemChanged?.Invoke(this, di.AttachedObject);
        }
        #endregion

        #region app logic and control data binding
        public void AttachData(object[] objects)
        {
            SearchList.Clear();
            foreach (object obj in objects)
                SearchList.Add(obj);
            Refresh();
        }

        public void AddItem(object obj)
        {
            SearchList.Add(obj);
            Refresh();
        }
        public void Refresh()
        {
            Sort();
            ItemsContainer.Children.Clear();
            foreach (object item in SearchList)
            {
                DropdownItem di = new(item);
                di.MouseEnter += TextBlock_MouseEnter;
                di.MouseLeave += TextBlock_MouseLeave;
                di.PreviewMouseLeftButtonDown += TextBlock_PreviewMouseLeftButtonDown;
                ItemsContainer.Children.Add(di);
            }
        }
        
        public void Sort()
        {
            for (int i = 0; i < SearchList.Count; i++)
            {
                for (int j = i; j < SearchList.Count; j++)
                {
                    if (!compareObjs(SearchList[i], SearchList[j]))
                    {
                        object temp = SearchList[i];
                        SearchList[i] = SearchList[j];
                        SearchList[j] = temp;
                    }
                }
            }
        }

        /// <summary>
        /// Checks if <paramref name="s1"/> is alphabetically in front of <paramref name="s2"/>
        /// </summary>
        private bool compareObjs(object o1, object o2)
        {
            if (o1 == null)
                return false;
            if (o2 == null)
                return true;

            string s1 = o1.ToString().ToLower();
            string s2 = o2.ToString().ToLower();

            int lenght = s1.Length < s2.Length ?
                         s1.Length : s2.Length;

            for(int i = 0; i < lenght; i++)
            {
                if (s1[i] < s2[i])
                    return true;
                else if (s1[i] > s2[i])
                    return false;
            }
            return s1.Length <= s2.Length;
        }

        private void loadTestData()
        {
            AddItem("Desibe");
            AddItem("Opalac");
            AddItem("Samo Ventilatori");
            AddItem("QA testers");
            AddItem("Frontend Developers");
            AddItem("Backend Developers");
            AddItem("Management Office");
            AddItem("Laptop Hoarders");
            AddItem("Music Listeners");
            AddItem("ABCDEFG");
            AddItem("Music");
            AddItem("Testing Team");
            AddItem("Marketing Team");
            AddItem("Tim Raketa");
            AddItem("Idemo nis");
            AddItem("Jagodina?");
            AddItem("Ducky One 2 mini");
            AddItem("You go to brazil");
            AddItem("Lorem Ipsum");
            AddItem("random string");
        }
        #endregion

        public void ResetControl()
        {
            object[] objs = new object[0];
            AttachData(objs);
        }

        #region Scroll
        private void Scroller_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            double marginTop = Scroller.VerticalOffset * Scroller.ViewportHeight / Scroller.ExtentHeight;
            if (double.IsNaN(marginTop)) //ako delimo nulom onda je bljak pa moramo da mu stavimo value na 0
                marginTop = 0;
            ScrollBorder.Margin = new Thickness(0, marginTop, 0, 0);

            double scrollbarHeight = Scroller.ViewportHeight * Scroller.ViewportHeight / Scroller.ExtentHeight;
            if (double.IsInfinity(scrollbarHeight)) //similar thing here
                scrollbarHeight = Scroller.ViewportHeight;

            ScrollBorder.Height = Scroller.ExtentHeight >= Scroller.ViewportHeight ? //ne pitaj zasto
                scrollbarHeight : Scroller.ExtentHeight;
        }
        #endregion

        #region UI Stuff
        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
            {
                Opacity = 0.5;
            }
            else
                Opacity = 1;
        }

        public void flickerWithRed()
        {
            DropdownButton.IsChecked = false;
            Border b1 = (Border)MainGrid.Children[0];
            Border b2 = (Border)MainGrid.Children[1];
            Utility.flickerPropertyWithRed(b1, BackgroundProperty, ((SolidColorBrush)b1.Background).Color);
            Utility.flickerPropertyWithRed(b2, BackgroundProperty, ((SolidColorBrush)b2.Background).Color);
        }
        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            InputText.MaxWidth = ActualWidth - ButtonBorder.ActualWidth;
        }
    }
}