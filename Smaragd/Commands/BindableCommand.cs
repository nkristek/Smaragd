using System;
using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// <see cref="ICommand"/> with <see cref="INotifyPropertyChanged"/> support
    /// </summary>
    public abstract class BindableCommand
        : ComputedBindableBase, ICommand, IRaiseCanExecuteChanged
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
        /// <see cref="ICommand.Execute(object)"/> implementation which executes <see cref="ExecuteSync(object)"/>
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
