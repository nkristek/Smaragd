using System;

namespace nkristek.MVVMBase.Commands
{
    /// <summary>
    /// RelayCommand implementation
    /// </summary>
    public sealed class RelayCommand
        : Command
    {
        private readonly Action<object> _execute;

        private readonly Predicate<object> _canExecute;

        public RelayCommand(Action<object> execute, Predicate<object> canExecute = null)
        {
            _execute = execute ?? throw new ArgumentNullException("execute");
            _canExecute = canExecute;
        }

        /// <summary>
        /// Indicates if <see cref="ExecuteSync(object)"/> is allowed to execute
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        public override bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : base.CanExecute(parameter);
        }

        /// <summary>
        /// Synchronously executes the given <see cref="Predicate{T}"/>
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        protected override void ExecuteSync(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
