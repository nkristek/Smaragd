using System.Windows.Input;

namespace nkristek.MVVMBase.Commands
{
    public interface IRaiseCanExecuteChanged: ICommand
    {
        /// <summary>
        /// Raises a CanExecuteChanged event
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
