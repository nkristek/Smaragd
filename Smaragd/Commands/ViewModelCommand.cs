using System;
using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="BindableCommand" /> with <see cref="ViewModel" /> support
    /// </summary>
    /// <typeparam name="TViewModel">Type of the parent ViewModel</typeparam>
    public abstract class ViewModelCommand<TViewModel>
        : BindableCommand where TViewModel : ViewModel
    {
        /// <inheritdoc />
        protected ViewModelCommand(TViewModel parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        private WeakReference<TViewModel> _parent;

        /// <summary>
        /// Parent of this <see cref="ViewModelCommand{TViewModel}"/>
        /// </summary>
        public TViewModel Parent
        {
            get
            {
                if (_parent != null && _parent.TryGetTarget(out var parent))
                    return parent;
                return null;
            }

            private set => _parent = value != null ? new WeakReference<TViewModel>(value) : null;
        }

        /// <inheritdoc />
        public sealed override bool CanExecute(object parameter)
        {
            return CanExecute(Parent, parameter);
        }

        /// <inheritdoc />
        protected sealed override void DoExecute(object parameter)
        {
            DoExecute(Parent, parameter);
        }

        /// <summary>
        /// Override this method to indicate if <see cref="DoExecute(TViewModel,object)"/> can execute
        /// </summary>
        /// <param name="viewModel"><see cref="Parent"/> of this command</param>
        /// <param name="parameter">Optional parameter</param>
        /// <returns>If the command can execute</returns>
        protected virtual bool CanExecute(TViewModel viewModel, object parameter)
        {
            return true;
        }

        /// <summary>
        /// Synchronous <see cref="ICommand.Execute(object)"/> which additionally includes context <see cref="ViewModel"/>
        /// </summary>
        /// <param name="viewModel"><see cref="Parent"/> of this command</param>
        /// <param name="parameter">Optional parameter</param>
        protected abstract void DoExecute(TViewModel viewModel, object parameter);
    }
}
