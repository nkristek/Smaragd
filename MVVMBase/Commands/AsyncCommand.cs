using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace nkristek.MVVMBase.Commands
{
    /// <summary>
    /// IAsyncCommand implementation
    /// </summary>
    public abstract class AsyncCommand
        : ICommand, IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Indicates if the command is working
        /// </summary>
        public bool IsWorking { get; private set; }

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
