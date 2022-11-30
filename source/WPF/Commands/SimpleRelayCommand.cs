using System;
using System.Windows.Input;

namespace File_Rename_Tool.Commands
{
    public class SimpleRelayCommand<T> : ICommand
    {
        private Action<T> m_command;
        private Func<bool> m_canExecute;

        public event EventHandler? CanExecuteChanged;

        public SimpleRelayCommand(Action<T> commandAction, Func<bool> canExecute = null)
        {
            m_command = commandAction;
            m_canExecute = canExecute;
        }

        public bool CanExecute(object? parameter)
        {
            return m_canExecute == null ? true : m_canExecute();
        }

        public void Execute(object? parameter)
        {
            if (parameter == null)
                return;
            if (m_command == null)
                return;

            if (typeof(T) == typeof(int))
            {
                parameter = int.Parse(parameter.ToString() ?? string.Empty);
            }

            m_command?.Invoke((T)parameter);
        }
    }
}
