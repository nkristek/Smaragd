using System;
using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// Synchronous <see cref="ICommand"/> with <see cref="INotifyPropertyChanged"/> support
    /// </summary>
    public abstract class BindableCommand
        : ComputedBindableBase, ICommand, IRaiseCanExecuteChanged
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
