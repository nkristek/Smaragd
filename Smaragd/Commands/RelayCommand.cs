using System;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="ICommand" /> which executes a given action
    /// </summary>
    public sealed class RelayCommand
        : Command
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        /// <inheritdoc />
        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <inheritdoc />
        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? base.CanExecute(parameter);
        }

        /// <inheritdoc />
        protected override void DoExecute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
