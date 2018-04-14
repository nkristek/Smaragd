using System;
using System.Threading.Tasks;
using System.Windows.Input;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Commands
{
    /// <summary>
    /// An asynchronous <see cref="ICommand"/> implementation with <see cref="INotifyPropertyChanged"/> support
    /// </summary>
    public abstract class AsyncBindableCommand
        : ComputedBindableBase, IAsyncCommand, IRaiseCanExecuteChanged
    {
        private bool _IsWorking;
        /// <summary>
        /// Indicates if <see cref="ExecuteAsync(object)"/> is running
        /// </summary>
        public bool IsWorking
        {
            get { return _IsWorking; }
            private set
            {
                if (SetProperty(ref _IsWorking, value))
                    RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Override this method to indicate if <see cref="Execute(object)"/> is allowed to execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter)
        {
            return !IsWorking;
        }

        /// <summary>
        /// This method is a requirement for <see cref="ICommand"/> and executes <see cref="ExecuteAsync(object)"/>
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        public async void Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        /// <summary>
        /// Execute this command asynchrously
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        /// <returns></returns>
        public async Task ExecuteAsync(object parameter)
        {
            try
            {
                IsWorking = true;
                await DoExecute(parameter);
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

        /// <summary>
        /// Asynchronous <see cref="ICommand.Execute(object)"/> implementation
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        /// <returns></returns>
        protected abstract Task DoExecute(object parameter);

        /// <summary>
        /// Will be called when <see cref="ExecuteAsync(object)"/> throws an <see cref="Exception"/>
        /// </summary>
        protected virtual void OnThrownException(object parameter, Exception exception) { }

        private EventHandler _internalCanExecuteChanged;

        /// <summary>
        /// This event will be raised when the result of <see cref="CanExecute(object)"/> probably changed and will need to be reevaluated
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add
            {
                _internalCanExecuteChanged += value;
            }

            remove
            {
                _internalCanExecuteChanged -= value;
            }
        }

        /// <summary>
        /// Raise an event that <see cref="CanExecute(object)"/> needs to be reevaluated
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            _internalCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
