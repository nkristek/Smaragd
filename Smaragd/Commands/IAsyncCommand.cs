using System.Threading.Tasks;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// Defines an asynchronous command.
    /// </summary>
    public interface IAsyncCommand
        : ICommand
    {
        /// <summary>
        /// Indicates if <see cref="ExecuteAsync"/> is working.
        /// </summary>
        bool IsWorking { get; }

        /// <inheritdoc cref="ICommand.Execute" />
        /// <returns>The <see cref="Task"/> of the asynchronous execution.</returns>
        Task ExecuteAsync(object parameter);
    }
}
