using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// Provides support for raising events on the <see cref="ICommand.CanExecuteChanged"/> event handler
    /// </summary>
    public interface IRaiseCanExecuteChanged: ICommand
    {
        /// <summary>
        /// Raise an event that <see cref="ICommand.CanExecute(object)"/> should be reevaluated
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
