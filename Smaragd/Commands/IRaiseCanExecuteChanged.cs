using System.Collections.Generic;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// Provides methods for raising events on the <see cref="ICommand.CanExecuteChanged"/> event handler.
    /// </summary>
    public interface IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Raise an event on <see cref="ICommand.CanExecuteChanged"/>, to indicate that <see cref="ICommand.CanExecute(object)"/> should be reevaluated.
        /// </summary>
        void RaiseCanExecuteChanged();

        /// <summary>
        /// Determine if <see cref="RaiseCanExecuteChanged"/> should be executed.
        /// This method should be called every time a property changes, to determine if <see cref="ICommand.CanExecute(object)"/> should be reevaluated.
        /// </summary>
        /// <param name="changedPropertyNames">Names of changed properties.</param>
        /// <returns><c>True</c> if <see cref="RaiseCanExecuteChanged"/> should be executed.</returns>
        bool ShouldRaiseCanExecuteChanged(IEnumerable<string> changedPropertyNames);
    }
}
