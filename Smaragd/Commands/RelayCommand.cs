using System;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// <see cref="ICommand"/> which executes a given action
    /// </summary>
    public sealed class RelayCommand
        : Command
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException(nameof(execute));
            _canExecute = canExecute;
        }

        /// <summary>
        /// Indicates if <see cref="DoExecute(object)"/> can execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? base.CanExecute(parameter);
        }

        /// <summary>
        /// Synchronously executes the given <see cref="Predicate{T}"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override void DoExecute(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
