using System.Windows;

namespace File_Rename_Tool
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            //Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("Themes/Dark.xaml", UriKind.Relative) });
            base.OnStartup(e);
        }
    }
}
