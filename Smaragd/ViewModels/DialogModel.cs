namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// A <see cref="ValidatingViewModel" /> which provides a title to use in dialogs
    /// </summary>
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
