using System;
using System.Threading.Tasks;
using System.Windows.Input;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Commands
{
    /// <summary>
    /// IAsyncCommand implementation with INotifyPropertyChanged support
    /// </summary>
    public abstract class AsyncBindableCommand
        : ComputedBindableBase, ICommand, IRaiseCanExecuteChanged
    {
        private bool _IsWorking;
        /// <summary>
        /// Indicates if the command is working
        /// </summary>
        public bool IsWorking
        {
            get
            {
                return _IsWorking;
            }

            private set
            {
                if (SetProperty(ref _IsWorking, value))
                    RaiseCanExecuteChanged();
            }
        }

        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        public async void Execute(object parameter)
        {
            try
            {
                IsWorking = true;
                await ExecuteAsync(parameter);
            }
            catch (Exception exception)
            {
                try
                {
                    OnThrownException(parameter, exception);
                }
                catch { }
            }
            finally
            {
                IsWorking = false;
            }
        }

        protected abstract Task ExecuteAsync(object parameter);

        /// <summary>
        /// Will be called when ExecuteAsync throws an exception
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
