using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for CleanDatePicker.xaml
    /// </summary>
    public partial class CleanDatePicker : UserControl
    {
        public event EventHandler<DateTime?> SelectedDateChanged;
        public CleanDatePicker()
        {
            InitializeComponent();
            DataContext = this;
        }

        #region Properties

        public string DisplayText
        {
            get => (string)GetValue(DisplayTextProperty);
            set => SetValue(DisplayTextProperty, value);
        }
        public static readonly DependencyProperty DisplayTextProperty
            = DependencyProperty.Register(
                "DisplayText",
                typeof(string),
                typeof(CleanDatePicker),
                new PropertyMetadata("Select Date")
                );

        public DateTime SelectedDate => (DateTime)DatePickerDialog.SelectedDate;

        public bool IsDateSelected {
            get => DatePickerDialog.SelectedDate != null;
            set
            {
                if(value)
                    DatePickerDialog.SelectedDate = DateTime.Now;
                else
                    DatePickerDialog.SelectedDate = null;
            }
        }

        #endregion

        #region Date Selection

        private void mainButton_Click(object sender, RoutedEventArgs e)
        {
            DatePickerDialog.IsDropDownOpen = true;
        }
        private void DatePickerDialog_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            if(DatePickerDialog.SelectedDate == null)
            {
                CalendarLabel.Text = DisplayText;
                CalendarGrid.Opacity = 0.5;
            }
            else
            {
                DateTime date = (DateTime)DatePickerDialog.SelectedDate;
                CalendarLabel.Text = date.ToString("dd.MM.yyyy");
                CalendarGrid.Opacity = 1;
            }
            SelectedDateChanged?.Invoke(this, DatePickerDialog.SelectedDate);
        }
        public void SelectDate(DateTime date)
        {
            DatePickerDialog.SelectedDate = date;
        }

        #endregion

        #region OnHover
        private void UserControl_MouseEnter(object sender, MouseEventArgs e)
        {
            CalendarGrid.Opacity = 1;
        }

        private void UserControl_MouseLeave(object sender, MouseEventArgs e)
        {
            if(DatePickerDialog.SelectedDate == null)
            {
                CalendarGrid.Opacity = 0.5;
            }
        }
        #endregion

        #region UI Stuff
        private void UserControl_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue == false)
                Opacity = 0.5;
            else
                Opacity = 1;
        }

        public void flickerWithRed()
        {
            Utility.flickerPropertyWithRed(CalendarBorder, BackgroundProperty, ((SolidColorBrush)CalendarBorder.Background).Color);
        }
        #endregion

    }
}
