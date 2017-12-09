using System;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// ViewModel implementation
    /// </summary>
    public abstract class ViewModel
        : ComputedBindableBase
    {
        public ViewModel(ViewModel parent = null, object view = null)
        {
            Parent = parent;
            View = view;
        }

        private bool _IsDirty;
        /// <summary>
        /// Indicates if a property changed on the ViewModel and the change is not persisted
        /// </summary>
        public bool IsDirty
        {
            get
            {
                return _IsDirty;
            }

            set
            {
                if (SetProperty(ref _IsDirty, value))
                {
                    if (Parent != null && value)
                        Parent.IsDirty = true;
                }
            }
        }

        private WeakReference<ViewModel> _Parent;
        /// <summary>
        /// The parent of this ViewModel
        /// </summary>
        public ViewModel Parent
        {
            get
            {
                if (_Parent != null && _Parent.TryGetTarget(out ViewModel parent))
                    return parent;
                return null;
            }

            set
            {
                if (Parent == value) return;
                _Parent = value != null ? new WeakReference<ViewModel>(value) : null;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// Gets the first parent of the requested type
        /// </summary>
        /// <typeparam name="T">Type of the requested parent</typeparam>
        /// <returns>The first parent of the requested type</returns>
        public T FirstParentOfType<T>() where T : ViewModel
        {
            if (_Parent != null && _Parent.TryGetTarget(out ViewModel parent))
            {
                return parent as T ?? parent.FirstParentOfType<T>();
            }
            return null;
        }

        /// <summary>
        /// The top most parent of this ViewModel
        /// </summary>
        public ViewModel TopMost
        {
            get
            {
                return Parent?.TopMost ?? this;
            }
        }

        private WeakReference<object> _View;
        /// <summary>
        /// The View of this ViewModel. Will return the view of the parent if no set.
        /// </summary>
        public object View
        {
            get
            {
                if (_View != null && _View.TryGetTarget(out object view))
                    return view;
                return Parent?.View;
            }

            set
            {
                if (View == value) return;
                _View = value != null ? new WeakReference<object>(value) : null;
                RaisePropertyChanged();
            }
        }

        /// <summary>
        /// This will set the IsDirty property to true is a property was changed, 
        /// It ignores IsDirty, Parent and View.
        /// Override if you want different behaviour.
        /// </summary>
        /// <param name="propertyName">Name of the property which was changed</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName != nameof(IsDirty) && propertyName != nameof(Parent) && propertyName != nameof(View))
                IsDirty = true;
        }
    }
}
