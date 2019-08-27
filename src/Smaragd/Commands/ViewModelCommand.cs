using System;
using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.Helpers;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="IViewModelCommand{TViewModel}" />
    public abstract class ViewModelCommand<TViewModel>
        : Bindable, IViewModelCommand<TViewModel> where TViewModel : class, IViewModel
    {
        private WeakReference<TViewModel>? _context;

        /// <inheritdoc />
        public TViewModel? Context
        {
            get => _context?.TargetOrDefault();
            set
            {
                if (!SetProperty(ref _context, value, out var oldValue))
                    return;

                if (oldValue is TViewModel oldViewModel)
                {
                    oldViewModel.PropertyChanging -= OnContextPropertyChanging;
                    oldViewModel.PropertyChanged -= OnContextPropertyChanged;
                }

                if (value is TViewModel newViewModel)
                {
                    newViewModel.PropertyChanging += OnContextPropertyChanging;
                    newViewModel.PropertyChanged += OnContextPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Gets called when a property value of the <see cref="Context"/> is changing.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        protected virtual void OnContextPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        /// <summary>
        /// Gets called when a property value of the <see cref="Context"/> changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        protected virtual void OnContextPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <inheritdoc />
        public bool CanExecute(object? parameter)
        {
            return CanExecute(Context, parameter);
        }

        /// <inheritdoc />
        public void Execute(object? parameter)
        {
            if (!CanExecute(parameter))
                return;

            Execute(Context, parameter);
        }

        /// <summary>
        /// Determines whether the command can execute based on its current state, the given <paramref name="viewModel"/> and <paramref name="parameter"/>.
        /// </summary>
        /// <param name="viewModel">The context <typeparamref name="TViewModel"/>.</param>
        /// <param name="parameter">Additional data used by the command. If the command does not require additional data to be passed, this parameter can be set to <see langword="null"/>.</param>
        /// <returns>Whether the command can execute based on its current state, the given <paramref name="viewModel"/> and <paramref name="parameter"/>.</returns>
        protected virtual bool CanExecute(TViewModel? viewModel, object? parameter = null)
        {
            return true;
        }

        /// <summary>
        /// Invoke the execution of this command.
        /// </summary>
        /// <param name="viewModel">The context <typeparamref name="TViewModel"/>.</param>
        /// <param name="parameter">Additional data used by the command. If the command does not require additional data to be passed, this parameter can be set to <see langword="null"/>.</param>
        protected abstract void Execute(TViewModel? viewModel, object? parameter = null);

        /// <inheritdoc />
        public event EventHandler CanExecuteChanged;

        /// <summary>
        /// Raise an event on <see cref="ICommand.CanExecuteChanged"/> to indicate that <see cref="ICommand.CanExecute(object)"/> should be reevaluated.
        /// </summary>
        protected virtual void NotifyCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}