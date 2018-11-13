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
    /// <summary>
    /// Synchronous <see cref="ICommand"/> with <see cref="INotifyPropertyChanged"/> support
    /// </summary>
    public abstract class BindableCommand
        : BindableBase, ICommand, IRaiseCanExecuteChanged
    {
        /// <inheritdoc />
        protected BindableCommand()
        {
            var canExecuteMethods = GetType().GetMethods(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public).Where(m => m.Name == nameof(CanExecute));
            var canExecuteSourceAttributes = canExecuteMethods.SelectMany(m => m.GetCustomAttributes<CanExecuteSourceAttribute>());
            _cachedCanExecuteSourceNames = canExecuteSourceAttributes.SelectMany(a => a.PropertySources).Distinct().ToList();
        }

        /// <inheritdoc />
        public virtual bool CanExecute(object parameter)
        {
            return true;
        }

        /// <inheritdoc />
        public void Execute(object parameter)
        {
            DoExecute(parameter);
        }

        /// <summary>
        /// Synchronous <see cref="ICommand.Execute(object)"/>
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        protected abstract void DoExecute(object parameter);

        /// <inheritdoc />
        public virtual event EventHandler CanExecuteChanged;

        /// <inheritdoc />
        public void RaiseCanExecuteChanged()
        {
            CanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }

        private readonly IList<string> _cachedCanExecuteSourceNames;
        
        /// <inheritdoc />
        public bool ShouldRaiseCanExecuteChanged(IEnumerable<string> changedPropertyNames)
        {
            return changedPropertyNames.Any(pn => _cachedCanExecuteSourceNames.Contains(pn));
        }
    }
}
