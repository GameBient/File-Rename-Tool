using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace File_Rename_Tool.Data
{
    public class FileData : INotifyPropertyChanged
    {
        private string m_titleNew;
        private string m_elementNameOriginal;
        private string m_elementNameReplaced;

        public string TitleNew
        {
            get => m_titleNew;
            set
            {
                if (value == m_titleNew) return;
                m_titleNew = value;
                OnPropertyChanged();
                TitleChanged?.Invoke(this);
            }
        }
        public string ElementNameOriginal => m_elementNameOriginal;
        public string ElementNameReplaced
        {
            get => m_elementNameReplaced;
            set
            {
                if (value == m_elementNameReplaced) return;
                m_elementNameReplaced = value;
                OnPropertyChanged();
            }
        }

        public string FileExtension { get; }

        public FileData(string elementName, string ext)
        {
            m_elementNameOriginal = elementName;
            m_elementNameReplaced = ElementNameOriginal;
            m_titleNew = string.Empty;

            FileExtension = ext;
        }

        public event Action<FileData>? TitleChanged;

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        public override string ToString()
        {
            return ElementNameOriginal;
        }
    }
}
