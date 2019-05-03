namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a <see cref="IViewModel" /> to use in a dialog.
    /// </summary>
    public interface IDialogModel
        : IViewModel
    {
        /// <summary>
        /// The title of the dialog.
        /// </summary>
        string Title { get; }
    }
}