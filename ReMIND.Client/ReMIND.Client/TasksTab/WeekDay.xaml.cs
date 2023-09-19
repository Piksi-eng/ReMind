using System.Windows;
using System.Windows.Controls;

namespace ReMIND.Client.TasksTab
{
    /// <summary>
    /// Interaction logic for WeekDay.xaml
    /// </summary>
    public partial class WeekDay : UserControl
    {
        public WeekDay()
        {
            InitializeComponent();
            DataContext = this;
        }
        public string DisplayText
        {
            get { return (string)GetValue(DisplayTextProperty);  }
            set
            {
                value = value.ToUpper();
                SetValue(DisplayTextProperty, value);
            }
        }
        public static readonly DependencyProperty DisplayTextProperty
            = DependencyProperty.Register(
                "DisplayText",
                typeof(string),
                typeof(WeekDay),
                new PropertyMetadata("MONDAY")
            );
    }
}
