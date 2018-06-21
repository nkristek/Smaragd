using System.Threading.Tasks;
using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <summary>
    /// An asynchronous <see cref="ICommand"/>
    /// </summary>
    public interface IAsyncCommand
        : ICommand
    {
        /// <summary>
        /// Indicates if <see cref="ExecuteAsync(object)"/> is working
        /// </summary>
        bool IsWorking { get; }

        /// <summary>
        /// Execute this command asynchrously
        /// </summary>
        /// <param name="parameter">Optional parameter</param>
        /// <returns>The <see cref="Task"/> of this execution</returns>
        Task ExecuteAsync(object parameter);
    }
}
