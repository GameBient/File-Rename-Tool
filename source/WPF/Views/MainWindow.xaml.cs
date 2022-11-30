using System.Windows;

namespace File_Rename_Tool.Views
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            this.DataContext = this;

            InitializeComponent();
        }

        private void Window_Drop(object sender, DragEventArgs e)
        {
            MainViewModel.FilesDroppedOnWindow(e);
        }
    }
}
