using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using File_Rename_Tool.Debugging;
using File_Rename_Tool.RegexOperations;
using File_Rename_Tool.Resources;
using File_Rename_Tool.ViewModels;

namespace File_Rename_Tool.Views
{
    public partial class MainView : UserControl, ILoggable
    {
        public MainView()
        {
            InitializeComponent();
        }

        public void OpenFolder()
        {
            using var folderBrowser = new System.Windows.Forms.FolderBrowserDialog();
            var browseResult = folderBrowser.ShowDialog();
            if (browseResult == System.Windows.Forms.DialogResult.OK)
            {
                string path = folderBrowser.SelectedPath;
                ((MainViewModel)DataContext).LoadElementsAtPath(path);
            }
        }

        string? GetInputFromDialog(Point position, string title = "Input", string context = "")
        {
            InputDialog inputDialog = new();
            inputDialog.Title = title;
            var location = PointToScreen(position);
            inputDialog.Left = location.X;
            inputDialog.Top = location.Y;
            inputDialog.Label = context;
            bool isOK = inputDialog.ShowDialog().GetValueOrDefault();
            if (!isOK) return null;
            string result = inputDialog.Input;
            return result;
        }

        public void FilesDroppedOnWindow(DragEventArgs e)
        {
            ((MainViewModel)DataContext).FilesDroppedOnWindow(e);
        }


        private void ListViewItem_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            ListViewItem item = (ListViewItem)sender;
            if (e.Key == Key.Down || e.Key == Key.Enter)
            {
                FocusNavigationDirection dir = FocusNavigationDirection.Down;
                item.MoveFocus(new(dir));
                e.Handled = true;
            }
            else if (e.Key == Key.Up)
            {
                FocusNavigationDirection dir = FocusNavigationDirection.Up;
                item.MoveFocus(new(dir));
                e.Handled = true;
            }
            else
            {
                e.Handled = false;
            }
        }

        private void TextBox_CustomContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            e.Handled = false;
            if (sender is not ContextMenu cm)
            {
                Log("ContextMenu is null");
                e.Handled = true;
                return;
            }
            // Reset ContextMenu
            cm.Items.Clear();
            cm.Items.Add(new MenuItem() { Command = ApplicationCommands.Copy });
            cm.Items.Add(new MenuItem() { Command = ApplicationCommands.Cut });
            cm.Items.Add(new MenuItem() { Command = ApplicationCommands.Paste });
            cm.Items.Add(new Separator());

            var target = cm.PlacementTarget;
            if (target is null)
                return;
            if (target == textBox_format)
                e.Handled = HandleFormatContextMenu(cm);
            else if (target == textBox_newName)
                e.Handled = HandleNewTitleContextMenu(cm);
        }

        private void ChooseFormat_Format_Button_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new();
            contextMenu.PlacementTarget = sender as Button;
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            HandleFormatContextMenu(contextMenu);
            contextMenu.IsOpen = true;
        }

        private void ChooseFormat_NewName_Button_Click(object sender, RoutedEventArgs e)
        {
            ContextMenu contextMenu = new();
            contextMenu.Placement = System.Windows.Controls.Primitives.PlacementMode.Bottom;
            contextMenu.PlacementTarget = sender as Button;
            HandleNewTitleContextMenu(contextMenu);
            contextMenu.IsOpen = true;
        }

        bool HandleFormatContextMenu(ContextMenu contextMenu)
        {
            foreach (var operation in GetVarOperation.Operations)
            {
                MenuItem menuItem = new();
                menuItem.Header = operation.Value.ReadableName;
                menuItem.Click += (sender, e) =>
                {
                    Point relativePoint = contextMenu.PlacementTarget.TransformToAncestor(this)
                          .Transform(new Point(0, 0));
                    string? input = GetInputFromDialog(relativePoint, operation.Value.ReadableName);
                    if (!string.IsNullOrEmpty(input))
                        textBox_format.SelectedText = $"<{operation.Key}={input}>";
                };
                contextMenu.Items.Add(menuItem);
            }

            return true;
        }

        bool HandleNewTitleContextMenu(ContextMenu contextMenu)
        {
            MenuItem? menuItem = null;
            // Variables found by RegexMatcher
            foreach (var variable in RegexMatcher.Variables)
            {
                menuItem = new();
                menuItem.Header = variable.Name;
                {
                    menuItem.Click += (sender, e) => textBox_newName.SelectedText = $"<{variable.Name}>";
                }
                contextMenu.Items.Add(menuItem);
            }
            // Title variable
            if (menuItem != null)
                contextMenu.Items.Add(new Separator());
            menuItem = new();
            menuItem.Header = Resource.custom_title;
            {
                menuItem.Click += (sender, e) => textBox_newName.SelectedText = $"<title>";
            }
            contextMenu.Items.Add(menuItem);
            return true;
        }

        // Debugging
        readonly Logger m_logger = new();
        bool m_debug = true;
        public void Log(object? context) { if (m_debug) m_logger.Log(context); }

        public void Log(string message) { if (m_debug) m_logger.Log(message); }

        public void Log(string message, object? context) { if (m_debug) m_logger.Log(message, context); }
    }
}
