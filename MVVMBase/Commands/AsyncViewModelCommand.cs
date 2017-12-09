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
            Parent = parent;
        }

        private WeakReference<TViewModel> _Parent;
        /// <summary>
        /// Parent of this AsyncViewModelCommand which is used when calling CanExecute/Execute or OnThrownException
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

        protected virtual bool CanExecute(TViewModel viewModel, object view, object parameter)
        {
            return true;
        }

        protected abstract Task ExecuteAsync(TViewModel viewModel, object view, object parameter);

        /// <summary>
        /// Will be called when ExecuteAsync throws an exception
        /// </summary>
        protected virtual void OnThrownException(TViewModel viewModel, object view, object parameter, Exception exception) { }

        public override sealed bool CanExecute(object parameter)
        {
            return !IsWorking && CanExecute(Parent, Parent?.View, parameter);
        }

        protected override sealed async Task ExecuteAsync(object parameter)
        {
            await ExecuteAsync(Parent, Parent?.View, parameter);
        }

        protected override sealed void OnThrownException(object parameter, Exception exception)
        {
            OnThrownException(Parent, Parent?.View, parameter, exception);
        }
    }
}
