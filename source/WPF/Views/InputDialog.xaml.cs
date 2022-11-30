using System;
using System.Windows;
using System.Windows.Input;

namespace File_Rename_Tool.Views
{
    public partial class InputDialog : Window
    {
        public InputDialog()
        {
            InitializeComponent();

            InputTextBox.Focus();
        }

        public static readonly DependencyProperty LabelProperty =
            DependencyProperty.Register("Label", typeof(string), typeof(InputDialog), new UIPropertyMetadata("Input:"));

        public static readonly DependencyProperty InputProperty =
            DependencyProperty.Register("Input", typeof(string), typeof(InputDialog));

        public static readonly DependencyProperty SubmitTextProperty =
            DependencyProperty.Register("SubmitText", typeof(string), typeof(InputDialog), new UIPropertyMetadata("OK"));


        public string Label
        {
            get { return (string)GetValue(LabelProperty); }
            set { SetValue(LabelProperty, value); }
        }

        public string Input
        {
            get { return (string)GetValue(InputProperty); }
            set { SetValue(InputProperty, value); }
        }

        public string SubmitText
        {
            get { return (string)GetValue(SubmitTextProperty); }
            set { SetValue(SubmitTextProperty, value); }
        }

        public bool IsActionButtonVisible => true;


        private void StatusActionButton_OnClick(object sender, RoutedEventArgs e)
        {
            InvokeAction();
        }

        private void InvokeAction()
        {
            DialogResult = true;
        }

        private void InputTextBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
                InvokeAction();
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
        }
    }
}
