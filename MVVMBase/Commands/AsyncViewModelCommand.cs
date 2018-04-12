using System;
using System.Threading.Tasks;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase.Commands
{
    /// <summary>
    /// AsyncBindableCommand implementation with ViewModel parameters in command methods
    /// </summary>
    /// <typeparam name="TViewModel">Type of the parent ViewModel</typeparam>
    public abstract class AsyncViewModelCommand<TViewModel>
        : AsyncBindableCommand where TViewModel : ViewModel
    {
        public AsyncViewModelCommand(TViewModel parent)
        {
            Parent = parent ?? throw new ArgumentNullException("parent");
        }

        private WeakReference<TViewModel> _Parent;
        /// <summary>
        /// Parent of this <see cref="AsyncViewModelCommand{TViewModel}"/> which will be used as the context <see cref="ViewModel"/>
        /// </summary>
        public TViewModel Parent
        {
            get
            {
                if (_Parent != null && _Parent.TryGetTarget(out TViewModel parent))
                    return parent;
                return null;
            }

            private set
            {
                if (Parent == value) return;
                _Parent = value != null ? new WeakReference<TViewModel>(value) : null;
            }
        }

        /// <summary>
        /// Override this method to indicate if <see cref="ExecuteAsync(TViewModel, object, object)"/> is allowed to execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected virtual bool CanExecute(TViewModel viewModel, object view, object parameter)
        {
            return true;
        }

        /// <summary>
        /// Asynchronous <see cref="ICommand.Execute(object)"/> which additionally includes context <see cref="ViewModel"/> and view
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected abstract Task ExecuteAsync(TViewModel viewModel, object view, object parameter);

        /// <summary>
        /// Will be called when <see cref="ExecuteAsync(TViewModel, object, object)"/> throws an <see cref="Exception"/>
        /// </summary>
        protected virtual void OnThrownException(TViewModel viewModel, object view, object parameter, Exception exception) { }

        /// <summary>
        /// This method returns the result of <see cref="CanExecute(TViewModel, object, object)"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override sealed bool CanExecute(object parameter)
        {
            return !IsWorking && CanExecute(Parent, Parent?.View, parameter);
        }

        /// <summary>
        /// This method executes <see cref="ExecuteAsync(TViewModel, object, object)"/>
        /// </summary>
        /// <param name="parameter"></param>
        public sealed override async Task ExecuteAsync(object parameter)
        {
            await ExecuteAsync(Parent, Parent?.View, parameter);
        }

        /// <summary>
        /// Will be called when <see cref="ExecuteAsync(TViewModel, object, object)"/> throws an <see cref="Exception"/>
        /// </summary>
        protected override sealed void OnThrownException(object parameter, Exception exception)
        {
            OnThrownException(Parent, Parent?.View, parameter, exception);
        }
    }
}
