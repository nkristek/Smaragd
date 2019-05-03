using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="IViewModelCommand{TViewModel}" />
    public abstract class ViewModelCommand<TViewModel>
        : Bindable, IViewModelCommand<TViewModel> where TViewModel : class, IViewModel
    {
        private readonly IList<string> _cachedCanExecuteSourceNames;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommand{TViewModel}" /> class.
        /// </summary>
        protected ViewModelCommand()
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

        /// <inheritdoc cref="ICommand.CanExecute" />
        protected virtual bool CanExecute(TViewModel viewModel, object parameter)
        {
            return true;
        }

        /// <inheritdoc cref="ICommand.Execute" />
        protected abstract void Execute(TViewModel viewModel, object parameter);

        /// <inheritdoc />
        public virtual event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        public virtual void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }
}