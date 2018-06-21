using System;
using System.Threading.Tasks;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Asynchronous implementation of <see cref="T:NKristek.Smaragd.Commands.RelayCommand" />
    /// </summary>
    public sealed class AsyncRelayCommand
        : AsyncCommand
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        /// <inheritdoc />
        public AsyncRelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <inheritdoc />
        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? base.CanExecute(parameter);
        }

        /// <inheritdoc />
        protected override async Task DoExecute(object parameter)
        {
            await Task.Run(() =>
            {
                _execute?.Invoke(parameter);
            });
        }
    }
}
