using System;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// <see cref="ICommand"/> implementation
    /// </summary>
    public abstract class Command
        : ICommand, IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Override this method to indicate if <see cref="Execute(object)"/> can execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <summary>
        /// Execute this command
        /// </summary>
        /// <param name="parameter"></param>
        public void Execute(object parameter)
        {
            DoExecute(parameter);
        }

        /// <summary>
        /// Synchronous <see cref="ICommand.Execute(object)"/>
        /// </summary>
        /// <param name="parameter"></param>
        protected abstract void DoExecute(object parameter);
        
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
