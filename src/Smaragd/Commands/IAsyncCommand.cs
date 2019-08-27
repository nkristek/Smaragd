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
        /// Indicates if the execute method is working.
        /// </summary>
        bool IsWorking { get; }

        /// <summary>
        /// Indicates if the execute method can be called concurrently.
        /// </summary>
        bool AllowsConcurrentExecution { get; }

        /// <inheritdoc cref="ICommand.Execute" />
        /// <returns>The <see cref="Task"/> of the asynchronous execution.</returns>
        Task ExecuteAsync(object? parameter = null);
    }
}