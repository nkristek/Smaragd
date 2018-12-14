using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Windows.Input;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="ICommand" />
    /// <summary>
    /// Implementation of <see cref="ICommand"/> with  support for <see cref="T:System.ComponentModel.INotifyPropertyChanged"/> and <see cref="IRaiseCanExecuteChanged"/>.
    /// </summary>
    /// <typeparam name="TViewModel">Type of the parent ViewModel.</typeparam>
    public abstract class ViewModelCommand<TViewModel>
        : Bindable, INamedCommand, IRaiseCanExecuteChanged where TViewModel : class, IViewModel
    {
        private readonly IList<string> _cachedCanExecuteSourceNames;

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewModelCommand{TViewModel}" /> class with its parent.
        /// </summary>
        /// <param name="parent">Parent of this <see cref="ViewModelCommand{TViewModel}" />.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="parent"/> is null.</exception>
        protected ViewModelCommand(TViewModel parent)
        {
            _parent = new WeakReference<TViewModel>(parent ?? throw new ArgumentNullException(nameof(parent)));

            var canExecuteMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public)
                .Where(m => m.Name == nameof(CanExecute));
            var canExecuteSourceAttributes = canExecuteMethods.SelectMany(m => m.GetCustomAttributes<CanExecuteSourceAttribute>());
            _cachedCanExecuteSourceNames = canExecuteSourceAttributes.SelectMany(a => a.PropertySources).Distinct().ToList();
        }

        /// <inheritdoc />
        /// <remarks>
        /// This defaults to the name of the type, including its namespace but not its assembly.
        /// </remarks>
        public virtual string Name => GetType().FullName;

        private readonly WeakReference<TViewModel> _parent;

        /// <summary>
        /// Parent of this <see cref="ViewModelCommand{TViewModel}"/>.
        /// </summary>
        public TViewModel Parent => _parent != null && _parent.TryGetTarget(out var parent) ? parent : null;

        /// <inheritdoc />
        public bool CanExecute(object parameter)
        {
            return CanExecute(Parent, parameter);
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
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

        /// <inheritdoc />
        public bool ShouldRaiseCanExecuteChanged(IEnumerable<string> changedPropertyNames)
        {
            return changedPropertyNames.Any(pn => _cachedCanExecuteSourceNames.Contains(pn));
        }
    }
}