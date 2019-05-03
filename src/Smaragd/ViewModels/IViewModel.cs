using System.Collections.Generic;
using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.Commands;

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
        /// Commands of this <see cref="IViewModel"/>.
        /// </summary>
        IReadOnlyDictionary<string, ICommand> Commands { get; }

        /// <summary>
        /// Adds the <paramref name="command"/> to the <see cref="Commands"/> dictionary.
        /// </summary>
        /// <param name="command">The command to add.</param>
        void AddCommand(INamedCommand command);

        /// <summary>
        /// Removes the <paramref name="command"/> from the <see cref="Commands"/> dictionary.
        /// </summary>
        /// <param name="command">The command to remove.</param>
        /// <returns>If the command was removed.</returns>
        bool RemoveCommand(INamedCommand command);
    }
}