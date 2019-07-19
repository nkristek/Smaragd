using System;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows.Input;
using NKristek.Smaragd.Helpers;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="IViewModelCommand{TViewModel}" />
    /// <remarks>
    /// This defines an asynchronous viewmodel command.
    /// </remarks>
    public abstract class AsyncViewModelCommand<TViewModel>
        : Bindable, IViewModelCommand<TViewModel>, IAsyncCommand where TViewModel : class, IViewModel
    {
        /// <inheritdoc />
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
        public virtual bool AllowsConcurrentExecution => false;

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return (!IsWorking || AllowsConcurrentExecution) && CanExecute(Parent, parameter);
        }

        /// <inheritdoc />
        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        private int _executionCount;
        
        private readonly object _executionCountLock = new object();

        private delegate void ReferenceAction<T>(ref T value);

        private void ModifyExecutionCount(ReferenceAction<int> modification)
        {
            if (modification == null)
                throw new ArgumentNullException(nameof(modification));

            NotifyPropertyChanging(nameof(IsWorking));
            lock (_executionCountLock)
                modification(ref _executionCount);
            NotifyPropertyChanged(nameof(IsWorking));

            if (!AllowsConcurrentExecution)
                NotifyCanExecuteChanged();
        }

        /// <inheritdoc />
        public bool IsWorking
        {
            get
            {
                lock (_executionCountLock)
                    return _executionCount > 0;
            }
        }
        
        private IDisposable BeginExecute()
        {
            ModifyExecutionCount((ref int executionCount) => executionCount++);
            return new ActionDisposable(EndExecute);
        }

        private void EndExecute()
        {
            ModifyExecutionCount((ref int executionCount) => executionCount--);
        }
        
        /// <inheritdoc />
        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            using (BeginExecute())
            {
                var task = ExecuteAsync(Parent, parameter);
                if (task != null)
                    await task;
            }
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
        /// Invoke the asynchronous execution of this command.
        /// </summary>
        /// <param name="viewModel">The context viewmodel.</param>
        /// <param name="parameter">Additional data used by the command. If the command does not require additional data to be passed, this parameter can be set to <see langword="null"/>.</param>
        /// <returns>The <see cref="Task"/> of the execution.</returns>
        protected abstract Task ExecuteAsync(TViewModel viewModel, object parameter);

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