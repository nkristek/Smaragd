namespace nkristek.MVVMBase.ViewModels
{
    public static class ViewModelExtensions
    {
        /// <summary>
        /// Gets the first parent of the requested <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of the requested parent</typeparam>
        /// <returns>The first parent of the requested type</returns>
        public static TViewModel FirstParentOfType<TViewModel>(this ViewModel viewModel) where TViewModel : ViewModel
        {
            var parent = viewModel?.Parent;
            return parent as TViewModel ?? parent?.FirstParentOfType<TViewModel>();
        }
    }
}
