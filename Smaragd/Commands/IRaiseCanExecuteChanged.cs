using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    public interface IRaiseCanExecuteChanged: ICommand
    {
        /// <summary>
        /// Raise an event that <see cref="ICommand.CanExecute(object)"/> should be reevaluated
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
