using System;
using System.ComponentModel;
using System.Linq.Expressions;
using System.Reflection;

namespace File_Rename_Tool.ViewModels
{
    public class PropertyChangeBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public void NotifyOfPropertyChange<T>(Expression<Func<T>> expression)
        {
            if (expression != null)
            {
                MemberInfo memberInfo = ((MemberExpression)expression.Body).Member;
                OnPropertyChanged(memberInfo.Name);
            }
        }

        protected void OnPropertyChanged(string propertyName)
        {
            System.Diagnostics.Debug.WriteLine("Property changed: " + propertyName);
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
