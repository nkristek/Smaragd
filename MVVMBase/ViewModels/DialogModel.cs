namespace nkristek.MVVMBase.ViewModels
{
    public abstract class DialogModel : ViewModel
    {
        private string _Title;
        /// <summary>
        /// Dialog title
        /// </summary>
        public string Title
        {
            get { return _Title; }
            set { SetProperty(ref _Title, value); }
        }
    }
}
