using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="INotifyPropertyChanged" />
    /// <summary>
    /// Defines properties which are useful for a ViewModel implementation.
    /// </summary>
    public interface IViewModel
        : IRaisePropertyChanging, IRaisePropertyChanged
    {
        /// <summary>
        /// Indicates if a property changed and is not persisted.
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
        /// Commands of this <see cref="IViewModel"/>.
        /// </summary>
        Dictionary<string, ICommand> Commands { get; }
    }
}