namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IDialogModel" />
    public abstract class DialogModel
        : ValidatingViewModel, IDialogModel
    {
        private string _title;

        /// <inheritdoc />
        public virtual string Title
        {
            get => _title;
            set => SetProperty(ref _title, value, out _);
        }
    }
}