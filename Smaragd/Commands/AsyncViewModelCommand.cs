using System;
using System.Threading.Tasks;
using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="AsyncBindableCommand" /> with <see cref="ViewModel" /> support
    /// </summary>
    /// <typeparam name="TViewModel">Type of the parent ViewModel</typeparam>
    public abstract class AsyncViewModelCommand<TViewModel>
        : AsyncBindableCommand where TViewModel : ViewModel
    {
        /// <inheritdoc />
        protected AsyncViewModelCommand(TViewModel parent)
        {
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        private WeakReference<TViewModel> _parent;

        /// <summary>
        /// Parent of this <see cref="AsyncViewModelCommand{TViewModel}"/>
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
        protected sealed override async Task DoExecute(object parameter)
        {
            await DoExecute(Parent, parameter);
        }

        /// <summary>
        /// Override this method to indicate if <see cref="DoExecute(TViewModel, object)"/> can execute
        /// </summary>
        /// <param name="viewModel"><see cref="Parent"/> of this command</param>
        /// <param name="parameter">Optional parameter</param>
        /// <returns>If the command can execute</returns>
        protected virtual bool CanExecute(TViewModel viewModel, object parameter)
        {
            return true;
        }

        /// <summary>
        /// Asynchronous <see cref="ICommand.Execute(object)"/> which additionally includes the context <see cref="ViewModel"/>
        /// </summary>
        /// <param name="viewModel"><see cref="Parent"/> of this command</param>
        /// <param name="parameter">Optional parameter</param>
        /// <returns>The <see cref="Task"/> of this execution</returns>
        protected abstract Task DoExecute(TViewModel viewModel, object parameter);
    }
}
