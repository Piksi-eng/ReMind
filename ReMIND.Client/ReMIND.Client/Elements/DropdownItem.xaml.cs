using System.Windows.Controls;

namespace ReMIND.Client.Elements
{
    /// <summary>
    /// Interaction logic for DropdownItem.xaml
    /// </summary>
    public partial class DropdownItem : UserControl
    {
        public object AttachedObject { get; set; }

        
        public string AttachedString => AttachedObject.ToString();

        public DropdownItem(object obj)
        {
            InitializeComponent();
            DataContext = this;
            AttachedObject = obj;
        }
    }
}
