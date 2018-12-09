using System.Collections.Generic;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// Defines a <see cref="T:NKristek.Smaragd.ViewModels.IViewModel" /> to use in a tree view.
    /// </summary>
    /// <remarks>
    /// It will update its parent <see cref="TreeViewModel" /> and children <see cref="TreeViewModel" /> with appropriate states for <see cref="IsChecked" />.
    /// To propagate changes to children, it is necessary that the <see cref="TreeChildren"/> property is overriden to return the appropriate collection.
    /// </remarks>
    public interface ITreeViewModel
        : IViewModel
    {
        /// <summary>
        /// If this <see cref="ITreeViewModel"/> is expanded.
        /// </summary>
        bool IsExpanded { get; }

        /// <summary>
        /// Children in the Tree. It is used to update the state of <see cref="IsChecked"/>.
        /// </summary>
        IEnumerable<ITreeViewModel> TreeChildren { get; }

        /// <summary>
        /// If this <see cref="ITreeViewModel"/> is checked. This property will get updated by children and updates its children when set.
        /// </summary>
        /// <remarks>
        /// A checkbox with three-state enabled will set <c>null</c> after the value was <c>true</c>. Since this is, most of the time, not the desired behaviour, setting this property directly to <c>null</c> will result in it being <c>false</c>. If you want to set <c>null</c>, use <see cref="SetIsChecked"/> instead.
        /// </remarks>
        bool? IsChecked { get; }

        /// <summary>
        /// Set the <see cref="IsChecked"/> property and optionally update <see cref="TreeChildren"/> and <see cref="ViewModel.Parent"/>. 
        /// </summary>
        /// <param name="value">The value that should be set.</param>
        /// <param name="updateChildren">If <see cref="TreeChildren"/> should be updated.</param>
        /// <param name="updateParent">If the <see cref="ViewModel.Parent"/> should be updated.</param>
        void SetIsChecked(bool? value, bool updateChildren, bool updateParent);
    }
}
