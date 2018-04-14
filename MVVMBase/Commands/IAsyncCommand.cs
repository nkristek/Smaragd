using System.Threading.Tasks;
using System.Windows.Input;

namespace nkristek.MVVMBase.Commands
{
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
        /// <returns></returns>
        Task ExecuteAsync(object parameter);
    }
}
