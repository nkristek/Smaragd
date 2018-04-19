namespace nkristek.MVVMBase.ViewModels
{
    public abstract class DialogModel : ViewModel
    {
        private string _title;
        /// <summary>
        /// Dialog title
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
    }
}
