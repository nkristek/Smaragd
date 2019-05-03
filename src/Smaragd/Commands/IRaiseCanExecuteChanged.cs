using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Provides methods for raising events on the <see cref="ICommand.CanExecuteChanged" /> event handler.
    /// </summary>
    public interface IRaiseCanExecuteChanged
        : ICommand
    {
        /// <summary>
        /// Raise an event on <see cref="ICommand.CanExecuteChanged"/>, to indicate that <see cref="ICommand.CanExecute(object)"/> should be reevaluated.
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}