namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <remarks>
    /// This class provides a title property to use in dialogs.
    /// </remarks>
    public abstract class DialogModel 
        : ValidatingViewModel
    {
        private string _title;

        /// <summary>
        /// The title of the dialog.
        /// </summary>
        public virtual string Title
        {
            get => _title;
            set => SetProperty(ref _title, value, out _);
        }
    }
}
