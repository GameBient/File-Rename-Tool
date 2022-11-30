using System.Windows;
using System.Windows.Controls;

namespace File_Rename_Tool.Controls
{
    /// <summary>
    /// Interaction logic for TextBoxMenuItem.xaml
    /// </summary>
    public partial class TextBoxMenuItem : MenuItem
    {
        public TextBoxMenuItem()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty LabelPropery =
            DependencyProperty.Register("Label", typeof(string), typeof(TextBoxMenuItem), new UIPropertyMetadata("Label"));

        public static readonly DependencyProperty TextBoxProperty =
            DependencyProperty.Register("Input", typeof(string), typeof(TextBoxMenuItem));

        public string Label
        {
            get { return (string)GetValue(LabelPropery); }
            set { SetValue(LabelPropery, value); }
        }

        public string Input
        {
            get { return (string)GetValue(TextBoxProperty); }
            set { SetValue(TextBoxProperty, value); }
        }
    }
}
