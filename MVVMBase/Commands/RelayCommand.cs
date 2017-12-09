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
            _execute = execute;
            _canExecute = canExecute;
        }

        public override bool CanExecute(object parameter)
        {
            return _canExecute != null ? _canExecute(parameter) : base.CanExecute(parameter);
        }

        protected override void ExecuteSync(object parameter)
        {
            _execute?.Invoke(parameter);
        }
    }
}
