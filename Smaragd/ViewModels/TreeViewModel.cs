using System.Collections.Generic;
using System.Linq;
using NKristek.Smaragd.Attributes;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <remarks>
    /// This class provides an <see cref="IsChecked" /> property to use in a TreeView.
    /// It will update its parent <see cref="TreeViewModel" /> and children <see cref="TreeViewModel" /> with appropriate states for <see cref="IsChecked" />.
    /// To propagate changes to children, it is necessary to override <see cref="TreeChildren"/> with the appropriate collection.
    /// </remarks>
    public abstract class TreeViewModel
        : ViewModel
    {
        private bool? _isChecked = false;

        /// <summary>
        /// <para>
        /// If this <see cref="TreeViewModel"/> is checked. This property will get updated by children and updates its children when set.
        /// </para>
        /// <para>
        /// A checkbox with three-state enabled will set <c>null</c> after the value was <c>true</c>. Since this is, most of the time, not the desired behaviour, setting this property directly to <c>null</c> will result in it being <c>false</c>. If you want to set <c>null</c>, use <see cref="SetIsChecked"/> instead.
        /// </para>
        /// </summary>
        public bool? IsChecked
        {
            get => _isChecked;
            set => SetIsChecked(value ?? false, true, true);
        }

        private bool _isExpanded;

        /// <summary>
        /// If this <see cref="TreeViewModel"/> is expanded.
        /// </summary>
        [IsDirtyIgnored]
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value, out _);
        }

        /// <summary>
        /// Set the <see cref="IsChecked"/> property and optionally update <see cref="TreeChildren"/> and <see cref="ViewModel.Parent"/>. 
        /// </summary>
        /// <param name="value">The value that should be set.</param>
        /// <param name="updateChildren">If <see cref="TreeChildren"/> should be updated.</param>
        /// <param name="updateParent">If the <see cref="ViewModel.Parent"/> should be updated.</param>
        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (!SetProperty(ref _isChecked, value, out _, nameof(IsChecked)))
                return;

            if (updateChildren && IsChecked.HasValue && TreeChildren != null)
                foreach (var child in TreeChildren)
                    child.SetIsChecked(IsChecked, true, false);

            if (updateParent)
                (Parent as TreeViewModel)?.ReevaluateIsChecked();
        }

        /// <summary>
        /// This reevaluates the <see cref="IsChecked"/> property based on the <see cref="TreeChildren"/> collection.
        /// </summary>
        protected void ReevaluateIsChecked()
        {
            if (TreeChildren == null)
                return;

            if (TreeChildren.All(c => c.IsChecked == true))
                SetIsChecked(true, false, true);
            else if (TreeChildren.All(c => c.IsChecked == false))
                SetIsChecked(false, false, true);
            else
                SetIsChecked(null, false, true);
        }

        /// <summary>
        /// Children in the Tree. It is used to update the state of <see cref="IsChecked"/>.
        /// </summary>
        protected virtual IEnumerable<TreeViewModel> TreeChildren { get; } = null;
    }
}
