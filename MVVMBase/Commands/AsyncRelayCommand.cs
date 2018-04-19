using System;
using System.Threading.Tasks;

namespace nkristek.MVVMBase.Commands
{
    /// <summary>
    /// Asynchronous implementation of a RelayCommand
    /// </summary>
    public sealed class AsyncRelayCommand
        : AsyncCommand
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        public AsyncRelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute;
            _canExecute = canExecute;
        }

        /// <summary>
        /// Indicates if <see cref="ExecuteAsync(object)"/> is allowed to execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            return _canExecute?.Invoke(parameter) ?? base.CanExecute(parameter);
        }

        /// <summary>
        /// Asynchronously executes the given <see cref="Predicate{T}"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override async Task DoExecute(object parameter)
        {
            await Task.Run(() =>
            {
                _execute?.Invoke(parameter);
            });
        }
    }
}
