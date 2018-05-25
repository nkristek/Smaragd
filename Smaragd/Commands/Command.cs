using System;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// ICommand implementation
    /// </summary>
    public abstract class Command
        : ICommand, IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Override this method to indicate if <see cref="Execute(object)"/> is allowed to execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// <see cref="ICommand.Execute(object)"/> implementation which executes <see cref="ExecuteSync(object)"/>
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            try
            {
                DoExecute(parameter);
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

        /// <summary>
        /// Synchronous <see cref="ICommand.Execute(object)"/>
        /// </summary>
        /// <param name="parameter"></param>
        protected abstract void DoExecute(object parameter);

        /// <summary>
        /// Will be called when <see cref="DoExecute(object)"/> throws an <see cref="Exception"/>
        /// </summary>
        protected virtual void OnThrownException(object parameter, Exception exception) { }

        private EventHandler _internalCanExecuteChanged;

        /// <summary>
        /// This event will be raised when the result of <see cref="CanExecute(object)"/> probably changed and will need to be reevaluated
        /// </summary>
        public event EventHandler CanExecuteChanged
        {
            add => _internalCanExecuteChanged += value;
            remove => _internalCanExecuteChanged -= value;
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
