namespace nkristek.MVVMBase.Commands
{
    public interface IRaiseCanExecuteChanged
    {
        /// <summary>
        /// Raises a CanExecuteChanged event
        /// </summary>
        void RaiseCanExecuteChanged();
    }
}
