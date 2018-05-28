using System;
using System.Windows.Input;

namespace CapsKey.Model
{
    // From https://docs.telerik.com/data-access/quick-start-scenarios/wpf/quickstart-wpf-viewmodelbase-and-relaycommand
    public class RelayCommand : ICommand
    {
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }

        private Action _methodToExecute;

        private Func<bool> _canExecuteEvaluator;

        public RelayCommand(Action methodToExecute, Func<bool> canExecuteEvaluator)
        {
            _methodToExecute = methodToExecute;
            _canExecuteEvaluator = canExecuteEvaluator;
        }

        public RelayCommand(Action methodToExecute)
            : this(methodToExecute, null)
        { }

        public bool CanExecute(object parameter)
        {
            if (_canExecuteEvaluator == null)
            {
                return true;
            }
            else
            {
                return _canExecuteEvaluator.Invoke();
            }
        }

        public void Execute(object parameter)
        {
            _methodToExecute.Invoke();
        }
    }
}