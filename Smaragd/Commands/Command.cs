using System;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="ICommand" />
    /// <summary>
    /// <see cref="T:System.Windows.Input.ICommand" /> implementation
    /// </summary>
    public abstract class Command
        : ICommand, IRaiseCanExecuteChanged
    {
        /// <inheritdoc />
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            DoExecute(parameter);
        }

        /// <summary>
        /// Synchronous <see cref="ICommand.Execute(object)"/>
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        protected abstract void DoExecute(object parameter);
        
        /// <inheritdoc />
        public virtual event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}
