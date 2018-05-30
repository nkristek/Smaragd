using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// Asynchronous <see cref="ICommand"/> with <see cref="INotifyPropertyChanged"/> support
    /// </summary>
    public abstract class AsyncBindableCommand
        : ComputedBindableBase, IAsyncCommand, IRaiseCanExecuteChanged
    {
        private bool _isWorking;

        /// <summary>
        /// Indicates if <see cref="ExecuteAsync(object)"/> is running
        /// </summary>
        public bool IsWorking
        {
            get => _isWorking;
            private set
            {
                if (SetProperty(ref _isWorking, value, out _))
                    RaiseCanExecuteChanged();
            }
        }

        /// <summary>
        /// Override this method to indicate if <see cref="Execute(object)"/> can execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter)
        {
            return !IsWorking;
        }

        /// <summary>
        /// This method executes <see cref="ExecuteAsync(object)"/>
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
            finally
            {
                IsWorking = false;
            }
        }

        /// <summary>
        /// Asynchronous <see cref="ICommand.Execute(object)"/>
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        /// <returns></returns>
        protected abstract Task DoExecute(object parameter);
        
        /// <summary>
        /// This event will be raised when the result of <see cref="CanExecute(object)"/> should be reevaluated
        /// </summary>
        public virtual event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raise an event that <see cref="CanExecute(object)"/> should be reevaluated
        /// </summary>
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
