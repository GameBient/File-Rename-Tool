using File_Rename_Tool.Commands;

namespace File_Rename_Tool.ViewModels
{
    public class ViewModelBase : PropertyChangeBase
    {
        public ViewModelBase()
        {
            RelayCommand = new(this);
        }

        public RelayCommand RelayCommand { get; private set; }
    }
}
