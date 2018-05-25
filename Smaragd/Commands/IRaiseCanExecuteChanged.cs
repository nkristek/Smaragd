using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    public interface IRaiseCanExecuteChanged: ICommand
    {
        /// <summary>
        /// Raises a CanExecuteChanged event
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
