using System;
using System.Windows.Input;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Commands
{
    /// <summary>
    /// ICommand implementation with INotifyPropertyChanged support
    /// </summary>
    public abstract class BindableCommand
        : ComputedBindableBase, ICommand, IRaiseCanExecuteChanged
    {
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public void Execute(object parameter)
        {
            try
            {
                ExecuteSync(parameter);
            }
            catch (Exception exception)
            {
                try
                {
                    OnThrownException(parameter, exception);
                }
                catch { }
            }
        }

        protected abstract void ExecuteSync(object parameter);

        /// <summary>
        /// Will be called when ExecuteSync throws an exception
        /// </summary>
        protected virtual void OnThrownException(object parameter, Exception exception) { }

        private EventHandler _internalCanExecuteChanged;

        public event EventHandler CanExecuteChanged
        {
            add
            {
                _internalCanExecuteChanged += value;
                CommandManager.RequerySuggested += value;
            }

            remove
            {
                _internalCanExecuteChanged -= value;
                CommandManager.RequerySuggested -= value;
            }
        }

        public void RaiseCanExecuteChanged()
        {
            _internalCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
