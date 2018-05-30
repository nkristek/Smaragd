namespace NKristek.Smaragd.ViewModels
{
    public abstract class DialogModel 
        : ValidatingViewModel
    {
        private string _title;

        /// <summary>
        /// Dialog title
        /// </summary>
        public string Title
        {
            get => _title;
            set => SetProperty(ref _title, value, out _);
        }
    }
}
