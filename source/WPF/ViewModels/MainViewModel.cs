using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;

using File_Rename_Tool.Data;
using File_Rename_Tool.Debugging;
using File_Rename_Tool.Resources;

namespace File_Rename_Tool.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        string? m_originalFormatString;
        string? m_newFormatString;
        bool m_isAutoPreviewEnabled;

        string? m_currentPath;
        FileData[]? m_files;


        public MainViewModel()
        {
            IsAutoPreviewEnabled = true;
            OriginalFormatString = "";
            NewFormatString = "";

            CurrentPath = Resource.default_path;
            Files = Array.Empty<FileData>();
        }

        public string OriginalFormatString
        {
            get => m_originalFormatString ?? "";
            set
            {
                if (value == m_originalFormatString) return;
                m_originalFormatString = value;
                NotifyOfPropertyChange(() => OriginalFormatString);
                if (IsAutoPreviewEnabled)
                    ApplyReplacementRules();
            }
        }

        public string NewFormatString
        {
            get => m_newFormatString ?? "";
            set
            {
                if (value == m_newFormatString) return;
                m_newFormatString = value;
                NotifyOfPropertyChange(() => NewFormatString);
                if (IsAutoPreviewEnabled)
                    ApplyReplacementRules();
            }
        }

        public bool IsAutoPreviewEnabled
        {
            get => m_isAutoPreviewEnabled;
            set
            {
                if (value == m_isAutoPreviewEnabled) return;
                m_isAutoPreviewEnabled = value;
                NotifyOfPropertyChange(() => IsAutoPreviewEnabled);
            }
        }

        public string CurrentPath
        {
            get => m_currentPath ?? "";
            set
            {
                if (value == m_currentPath) return;
                m_currentPath = value;
                NotifyOfPropertyChange(() => CurrentPath);
            }
        }

        public FileData[] Files
        {
            get => m_files ?? Array.Empty<FileData>();
            set
            {
                if (value == m_files) return;
                m_files = value;
                NotifyOfPropertyChange(() => Files);
            }
        }


        public void FilesDroppedOnWindow(DragEventArgs e)
        {
            string[] folders = (string[])e.Data.GetData(DataFormats.FileDrop);
            e.Handled = true;
            if (folders == null)
                return;
            if (folders.Length == 0)
                return;
            Log("Dropped:\n\t" + string.Join("\n\t", folders));
            if (Directory.Exists(folders[0]))
                LoadElementsAtPath(folders[0]);
        }


        public void RenameFiles()
        {
            if (Files.Length == 0)
                return;
            ApplyReplacementRules();
            if (!AreAllNamesValid())
            {
                StringBuilder infoString = new(10);
                infoString.AppendLine(Resource.info_message_rename_failed);
                infoString.AppendLine(string.Join(" ", invalidChars));
                MessageBox.Show(infoString.ToString(), Resource.info_title_rename_failed);
                return;
            }
            foreach (var item in Files)
            {
                string originalPath = Path.Combine(CurrentPath, item.ElementNameOriginal + item.FileExtension);
                string newPath = Path.Combine(CurrentPath, item.ElementNameReplaced + item.FileExtension);
                File.Move(originalPath, newPath);
            }
            LoadElementsAtPath(CurrentPath);
            MessageBox.Show(Resource.info_title_rename_successful);
        }

        readonly char[] invalidChars = { '?', '\\', '/', '"', ':', '<', '>', '|', '*' };
        bool AreAllNamesValid()
        {
            bool nameIsValid = true;
            foreach (var item in Files)
            {
                if (nameIsValid)
                {
                    nameIsValid = Files.Where(elem => elem != item).All(elem => elem.ElementNameReplaced != item.ElementNameReplaced);
                    nameIsValid = nameIsValid && item.ElementNameReplaced.All(symbol => invalidChars.All(x => x != symbol));
                }
                if (item.ElementNameReplaced == string.Empty)
                    nameIsValid = false;
            }
            return nameIsValid;
        }

        public void OpenFolder()
        {
            var dlg = new System.Windows.Forms.FolderBrowserDialog();
            var res = dlg.ShowDialog();
            if (res == System.Windows.Forms.DialogResult.OK)
            {
                string path = dlg.SelectedPath;
                LoadElementsAtPath(path);
            }
        }

        public void ApplyReplacementRules()
        {
            foreach (var item in Files)
                ApplyChangesToItem(item);
        }

        public void LoadElementsAtPath(string path)
        {
            CurrentPath = path;

            string[] fileNames = Directory.GetFiles(path);
            foreach (var item in Files)
                item.TitleChanged -= Episode_TitleChanged;

            Files = new FileData[fileNames.Length];
            for (int i = 0; i < Files.Length; i++)
            {
                Files[i] = new FileData(
                    Path.GetFileNameWithoutExtension(fileNames[i]),
                    Path.GetExtension(fileNames[i]));
                Files[i].TitleChanged += Episode_TitleChanged;
            }

            if (Files.Length > 0 && string.IsNullOrEmpty(OriginalFormatString))
                OriginalFormatString = Files[0].ElementNameOriginal;
            ApplyReplacementRules();
        }

        private void Episode_TitleChanged(FileData fileData)
        {
            if (IsAutoPreviewEnabled)
                ApplyChangesToItem(fileData);
        }

        void ApplyChangesToItem(FileData fileData)
        {
            //StringBuilder sb = new();

            string changedName = fileData.ElementNameOriginal;
            if (OriginalFormatString.Length != 0)
            {
                //sb.AppendLine("\nChanging fileData " + Array.FindIndex(Files, x => x == fileData) + "\n-----------------");
                changedName = RegexMatcher.ChangeTextWithVar(OriginalFormatString, NewFormatString, changedName, fileData.TitleNew);
                //sb.AppendLine("Item Name is now " + changedName);
            }
            //Log(sb.ToString());

            fileData.ElementNameReplaced = changedName;
        }

        // Debugging
        readonly Logger m_logger = new();
        bool m_debug = true;
        public void Log(object? context) { if (m_debug) m_logger.Log(context); }

        public void Log(string message) { if (m_debug) m_logger.Log(message); }

        public void Log(string message, object? context) { if (m_debug) m_logger.Log(message, context); }
    }
}
