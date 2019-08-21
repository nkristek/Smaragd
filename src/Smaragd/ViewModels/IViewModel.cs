namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Defines properties which are useful for a viewmodel implementation.
    /// </summary>
    public interface IViewModel
        : IValidatingBindable
    {
        /// <summary>
        /// Indicates if a property changed and the change is not persisted.
        /// </summary>
        bool IsDirty { get; set; }

        /// <summary>
        /// The parent of this <see cref="IViewModel"/>.
        /// </summary>
        IViewModel Parent { get; set; }

        /// <summary>
        /// Indicates if this <see cref="IViewModel"/> instance is read only and it is not possible to change a property value.
        /// </summary>
        bool IsReadOnly { get; set; }

        /// <summary>
        /// Indicates if this <see cref="IViewModel"/> instance is currently being updated.
        /// </summary>
        bool IsUpdating { get; set; }
        
        /// <summary>
        /// If data is valid.
        /// </summary>
        bool IsValid { get; }
    }
}