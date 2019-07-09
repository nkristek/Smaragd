using System.Collections.Generic;
using System.Linq;
using NKristek.Smaragd.Attributes;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="ITreeViewModel" />
    public abstract class TreeViewModel
        : ViewModel, ITreeViewModel
    {
        private bool _isExpanded;

        /// <inheritdoc />
        [IsDirtyIgnored]
        [IsReadOnlyIgnored]
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        /// <inheritdoc />
        public virtual IEnumerable<ITreeViewModel> TreeChildren { get; } = null;

        private bool? _isChecked = false;

        /// <inheritdoc />
        public bool? IsChecked
        {
            get => _isChecked;
            set => SetIsChecked(value ?? false, true, true);
        }

        /// <inheritdoc />
        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent)
        {
            if (!SetProperty(ref _isChecked, value, null, nameof(IsChecked)))
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
            if (TreeChildren == null || !TreeChildren.Any())
                return;

            if (TreeChildren.All(c => c.IsChecked == true))
                SetIsChecked(true, false, true);
            else if (TreeChildren.All(c => c.IsChecked == false))
                SetIsChecked(false, false, true);
            else
                SetIsChecked(null, false, true);
        }
    }
}