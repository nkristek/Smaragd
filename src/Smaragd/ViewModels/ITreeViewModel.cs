using System.Collections.Generic;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a <see cref="IViewModel" /> to use in a tree view.
    /// </summary>
    public interface ITreeViewModel
        : IViewModel
    {
        /// <summary>
        /// If this <see cref="ITreeViewModel"/> is expanded.
        /// </summary>
        bool IsExpanded { get; set; }

        /// <summary>
        /// The children in the tree. It is also used to update the state of <see cref="IsChecked"/>.
        /// </summary>
        IEnumerable<ITreeViewModel>? TreeChildren { get; }

        /// <summary>
        /// If this <see cref="ITreeViewModel"/> is checked. This property will get updated by its children and updates the children when set.
        /// </summary>
        /// <remarks>
        /// A checkbox with three-state enabled will set <see langword="null"/> after the value was <see langword="true"/>.
        /// Since this is, most of the time, not the desired behaviour, setting this property directly to <see langword="null"/> will result in it being <see langword="false"/>.
        /// If you want to set <see langword="null"/>, use <see cref="SetIsChecked"/> instead.
        /// </remarks>
        bool? IsChecked { get; set; }

        /// <summary>
        /// Set the <see cref="IsChecked"/> property and optionally update <see cref="TreeChildren"/> and <see cref="IViewModel.Parent"/>. 
        /// </summary>
        /// <param name="value">The value that should be set.</param>
        /// <param name="updateChildren">If <see cref="TreeChildren"/> should be updated.</param>
        /// <param name="updateParent">If the <see cref="ViewModel.Parent"/> should be updated.</param>
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent);
    }
}