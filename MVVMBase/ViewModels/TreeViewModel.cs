using System.Collections.ObjectModel;
using System.Linq;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// This <see cref="ViewModel"/> provides an <see cref="IsChecked"/> implementation to use in a TreeView. It will update its parent <see cref="TreeViewModel"/> and children <see cref="TreeViewModel"/> with appropriate states for <see cref="IsChecked"/>.
    /// </summary>
    public abstract class TreeViewModel
        : ViewModel
    {
        private bool? _isChecked;
        /// <summary>
        /// If this <see cref="TreeViewModel"/> is checked. This property will get updated by children and updates its children when set.
        /// </summary>
        public bool? IsChecked
        {
            get => _isChecked;
            set => SetIsChecked(value, true, true);
        }

        private bool _isExpanded;
        /// <summary>
        /// If this <see cref="TreeViewModel"/> is expanded in the tree.
        /// </summary>
        public bool IsExpanded
        {
            get => _isExpanded;
            set => SetProperty(ref _isExpanded, value);
        }

        /// <summary>
        /// A collection of all owned <see cref="TreeViewModel"/>.
        /// </summary>
        protected ObservableCollection<TreeViewModel> OwnedElements { get; } = new ObservableCollection<TreeViewModel>();

        private ReadOnlyObservableCollection<TreeViewModel> _children;
        /// <summary>
        /// Children of this <see cref="TreeViewModel"/>. This property wraps the protected <see cref="OwnedElements"/> collection.
        /// </summary>
        public ReadOnlyObservableCollection<TreeViewModel> Children => _children ?? (_children = new ReadOnlyObservableCollection<TreeViewModel>(OwnedElements));

        /// <summary>
        /// Set <see cref="IsChecked"/> property and optionally update <see cref="Children"/> and <see cref="ViewModel.Parent"/>. 
        /// </summary>
        /// <param name="value">The value that should be set.</param>
        /// <param name="updateChildren">If <see cref="Children"/> should be updated.</param>
        /// <param name="updateParent">If the <see cref="ViewModel.Parent"/> should be updated.</param>
        /// <param name="allowNull">If this is false, only true and false will be set. If <paramref name="value"/> is null, it will default to false. If this is true, <paramref name="value"/> will be set with no changes.</param>
        public void SetIsChecked(bool? value, bool updateChildren, bool updateParent, bool allowNull = false)
        {
            if (!allowNull && value == null)
                value = false;

            if (!SetProperty(ref _isChecked, value, nameof(IsChecked)))
                return;

            if (updateChildren && IsChecked.HasValue)
                foreach (var child in Children)
                    child.SetIsChecked(IsChecked, true, false);

            if (updateParent)
                (Parent as TreeViewModel)?.ReevaluateIsChecked();
        }

        /// <summary>
        /// This reevaluates the <see cref="IsChecked"/> property based on the <see cref="Children"/> collection.
        /// </summary>
        protected void ReevaluateIsChecked()
        {
            if (Children.All(c => c.IsChecked == true))
            {
                SetIsChecked(true, false, true);
                return;
            };

            if (Children.All(c => c.IsChecked == false))
            {
                SetIsChecked(false, false, true);
                return;
            };

            SetIsChecked(null, false, true, true);
        }
    }
}
