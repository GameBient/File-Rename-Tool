using System;
using System.Reflection;
using System.Windows.Input;

namespace File_Rename_Tool.Commands
{
    public class RelayCommand : ICommand
    {
        private readonly Type m_type;
        private object m_model;

        public event EventHandler? CanExecuteChanged;

        public RelayCommand(object model)
        {
            m_model = model;
            m_type = model.GetType();
        }

        public bool CanExecute(object? parameter)
        {
            return true;
        }

        public void Execute(object? parameter)
        {
            string? action = parameter as string;
            if (string.IsNullOrEmpty(action))
                return;
            string parameterData = string.Empty;
            if (action.Contains(","))
            {
                parameterData = action.Split(',')[1];
                action = action.Split(',')[0];
            }
            MethodInfo? method = m_type.GetMethod(action);
            if (method != null)
            {
                try
                {
                    if (method.GetParameters().Length == 0)
                    {
                        method.Invoke(m_model, null);
                    }
                    else
                    {

                    }
                }
                catch (Exception e)
                {
                    if (e.InnerException != null)
                        throw e.InnerException;
                    throw;
                }
            }
        }
    }
}
