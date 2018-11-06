using System.Collections.Generic;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <summary>
    /// Provides support for raising events on the <see cref="ICommand.CanExecuteChanged"/> event handler
    /// </summary>
    public interface IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Raise an event that <see cref="ICommand.CanExecute(object)"/> should be reevaluated
        /// </summary>
        void RaiseCanExecuteChanged();

        /// <summary>
        /// Determine if <see cref="RaiseCanExecuteChanged"/> should be executed.
        /// </summary>
        /// <param name="changedPropertyNames">Names of changed properties.</param>
        /// <returns><c>True</c> if <see cref="RaiseCanExecuteChanged"/> should be executed.</returns>
        bool ShouldRaiseCanExecuteChanged(IEnumerable<string> changedPropertyNames);
    }
}
