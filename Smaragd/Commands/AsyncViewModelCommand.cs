using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Input;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="IAsyncCommand" />
    /// <summary>
    /// Implementation of <see cref="IAsyncCommand"/> with support for <see cref="T:System.ComponentModel.INotifyPropertyChanged"/> and <see cref="IRaiseCanExecuteChanged"/>.
    /// </summary>
    /// <typeparam name="TViewModel">Type of the parent ViewModel.</typeparam>
    public abstract class AsyncViewModelCommand<TViewModel>
        : Bindable, IAsyncCommand, IRaiseCanExecuteChanged where TViewModel : ViewModel
    {
        private readonly IList<string> _cachedCanExecuteSourceNames;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="AsyncViewModelCommand{TViewModel}" /> class with its parent.
        /// </summary>
        /// <param name="parent">Parent of this <see cref="AsyncViewModelCommand{TViewModel}" />.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="parent"/> is null.</exception>
        protected AsyncViewModelCommand(TViewModel parent)
        {
            _parent = new WeakReference<TViewModel>(parent ?? throw new ArgumentNullException(nameof(parent)));

            var canExecuteMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(m => m.Name == nameof(CanExecute));
            var canExecuteSourceAttributes = canExecuteMethods.SelectMany(m => m.GetCustomAttributes<CanExecuteSourceAttribute>());
            _cachedCanExecuteSourceNames = canExecuteSourceAttributes.SelectMany(a => a.PropertySources).Distinct().ToList();
        }

        /// <summary>
        /// Name of this <see cref="AsyncViewModelCommand{TViewModel}"/>.
        /// </summary>
        /// <remarks>
        /// This defaults to the name of the type, including its namespace but not its assembly.
        /// </remarks>
        public virtual string Name => GetType().FullName;

        private readonly WeakReference<TViewModel> _parent;

        /// <summary>
        /// Parent of this <see cref="AsyncViewModelCommand{TViewModel}"/>.
        /// </summary>
        public TViewModel Parent => _parent != null && _parent.TryGetTarget(out var parent) ? parent : null;

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
        public bool CanExecute(object parameter)
        {
            return !IsWorking && CanExecute(Parent, parameter);
        }

        /// <inheritdoc />
        async void ICommand.Execute(object parameter)
        {
            await ExecuteAsync(parameter);
        }

        /// <inheritdoc />
        public async Task ExecuteAsync(object parameter)
        {
            try
            {
                IsWorking = true;
                await ExecuteAsync(Parent, parameter);
            }
            finally
            {
                IsWorking = false;
            }
        }

        /// <inheritdoc cref="IAsyncCommand.CanExecute" />
        protected virtual bool CanExecute(TViewModel viewModel, object parameter)
        {
            return true;
        }

        /// <inheritdoc cref="IAsyncCommand.ExecuteAsync" />
        protected abstract Task ExecuteAsync(TViewModel viewModel, object parameter);
        
        /// <inheritdoc />
        public virtual event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
        
        /// <inheritdoc />
        public bool ShouldRaiseCanExecuteChanged(IEnumerable<string> changedPropertyNames)
        {
            return changedPropertyNames.Any(pn => _cachedCanExecuteSourceNames.Contains(pn));
        }
    }
}
