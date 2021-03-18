using System;
using System.Windows.Input;

namespace App34
{
    public class DelegateCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        Action _action;
        public DelegateCommand(Action action)
        {
            _action = action;
        }

        public bool CanExecute(object parameter = null)
        {
            return _action != null;
        }

        public void Execute(object parameter = null)
        {
            _action.Invoke();
        }
    }
}
