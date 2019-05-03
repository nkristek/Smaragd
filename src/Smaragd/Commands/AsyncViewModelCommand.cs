using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using NKristek.Smaragd.Attributes;
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
        private readonly IList<string> _cachedCanExecuteSourceNames;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncViewModelCommand{TViewModel}" /> class.
        /// </summary>
        protected AsyncViewModelCommand()
        {
            _cachedCanExecuteSourceNames = GetCanExecuteSourceNames().ToList();
        }

        private IEnumerable<string> GetCanExecuteSourceNames()
        {
            return GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.Name == nameof(CanExecute))
                .SelectMany(m => m.GetCustomAttributes<CanExecuteSourceAttribute>())
                .SelectMany(a => a.PropertySources)
                .Distinct();
        }

        /// <inheritdoc />
        /// <remarks>
        /// This defaults to the name of the type, including its namespace but not its assembly.
        /// </remarks>
        public virtual string Name => GetType().FullName;

        private WeakReference<TViewModel> _parent;

        /// <inheritdoc />
        public TViewModel Parent
        {
            get => _parent != null && _parent.TryGetTarget(out var parent) ? parent : null;
            set
            {
                if (value == Parent) return;
                var oldValue = Parent;
                if (oldValue != null)
                    oldValue.PropertyChanged -= ParentOnPropertyChanged;

                _parent = value != null ? new WeakReference<TViewModel>(value) : null;
                RaisePropertyChanged();

                if (value != null)
                    value.PropertyChanged += ParentOnPropertyChanged;
            }
        }

        private void ParentOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (_cachedCanExecuteSourceNames.Contains(e.PropertyName))
                RaiseCanExecuteChanged();
        }

        private bool _isWorking;

        /// <inheritdoc />
        public bool IsWorking
        {
            get => _isWorking;
            private set
            {
                if (SetProperty(ref _isWorking, value, out _))
                    RaiseCanExecuteChanged();
            }
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

        private int _isWorkingCount;

        private readonly object _isWorkingLock = new object();

        /// <inheritdoc />
        public async Task ExecuteAsync(object parameter)
        {
            if (!CanExecute(parameter))
                return;

            try
            {
                lock (_isWorkingLock)
                {
                    _isWorkingCount++;
                    IsWorking = true;
                }

                await ExecuteAsync(Parent, parameter);
            }
            finally
            {
                lock (_isWorkingLock)
                {
                    _isWorkingCount--;
                    IsWorking = _isWorkingCount > 0;
                }
            }
        }

        /// <inheritdoc cref="ICommand.CanExecute" />
        protected virtual bool CanExecute(TViewModel viewModel, object parameter)
        {
            return true;
        }

        /// <inheritdoc cref="IAsyncCommand.ExecuteAsync" />
        protected abstract Task ExecuteAsync(TViewModel viewModel, object parameter);

        /// <inheritdoc />
        public virtual event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}