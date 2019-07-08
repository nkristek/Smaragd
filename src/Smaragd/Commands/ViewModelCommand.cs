using System;
using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="IViewModelCommand{TViewModel}" />
    public abstract class ViewModelCommand<TViewModel>
        : Bindable, IViewModelCommand<TViewModel> where TViewModel : class, IViewModel
    {
        /// <inheritdoc />
        /// <remarks>
        /// This defaults to the name of the type, including its namespace but not its assembly.
        /// </remarks>
        public virtual string Name => GetType().FullName;

        private WeakReference<TViewModel> _parent;

        /// <inheritdoc />
        public TViewModel Parent
        {
            get => _parent?.TargetOrDefault();
            set
            {
                if (!SetProperty(ref _parent, value, out var oldValue))
                    return;

                if (oldValue != null)
                {
                    oldValue.PropertyChanging -= OnParentPropertyChanging;
                    oldValue.PropertyChanged -= OnParentPropertyChanged;
                }

                if (value != null)
                {
                    value.PropertyChanging += OnParentPropertyChanging;
                    value.PropertyChanged += OnParentPropertyChanged;
                }
            }
        }

        /// <summary>
        /// Gets called when a property value of the <see cref="Parent"/> is changing.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        protected virtual void OnParentPropertyChanging(object sender, PropertyChangingEventArgs e)
        {
        }

        /// <summary>
        /// Gets called when a property value of the <see cref="Parent"/> changed.
        /// </summary>
        /// <param name="sender">The sender of the event.</param>
        /// <param name="e">Arguments of the event.</param>
        protected virtual void OnParentPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return CanExecute(Parent, parameter);
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            if (!CanExecute(Parent, parameter))
                return;

            Execute(Parent, parameter);
        }

        /// <summary>
        /// Determines whether the command can execute based on its current state and the given <paramref name="viewModel"/>.
        /// </summary>
        /// <param name="viewModel">The context viewmodel.</param>
        /// <param name="parameter">Additional data used by the command. If the command does not require additional data to be passed, this parameter can be set to <see langword="null"/>.</param>
        /// <returns>Whether the command can execute based on its current state and the given <paramref name="viewModel"/>.</returns>
        protected virtual bool CanExecute(TViewModel viewModel, object parameter)
        {
            return true;
        }

        /// <summary>
        /// Invoke the execution of this command.
        /// </summary>
        /// <param name="viewModel">The context viewmodel.</param>
        /// <param name="parameter">Additional data used by the command. If the command does not require additional data to be passed, this parameter can be set to <see langword="null"/>.</param>
        protected abstract void Execute(TViewModel viewModel, object parameter);

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