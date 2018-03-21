using System;
using System.Runtime.CompilerServices;

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
        /// Indicates if a property changed on the <see cref="ViewModel"/> and the change is not persisted
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

        private bool _IsReadOnly;
        /// <summary>
        /// Indicates if this <see cref="ViewModel"/> instance is read only and it is not possible to change a property value
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return _IsReadOnly;
            }

            set
            {
                SetProperty(ref _IsReadOnly, value);
            }
        }

        private WeakReference<ViewModel> _Parent;
        /// <summary>
        /// The parent of this <see cref="ViewModel"/>
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
        /// Gets the first parent of the requested <typeparamref name="T"/>
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
        /// The top most parent of this <see cref="ViewModel"/>
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
        /// The View of this <see cref="ViewModel"/>. Will return the view of the parent if no set.
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
        /// This will set the <see cref="IsDirty"/> property to true is a property was changed, 
        /// It ignores <see cref="IsDirty"/>, <see cref="Parent"/>, <see cref="View"/> and <see cref="IsReadOnly"/>.
        /// Override if you want different behaviour.
        /// </summary>
        /// <param name="propertyName">Name of the property which was changed</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (propertyName != nameof(IsDirty)
             && propertyName != nameof(Parent)
             && propertyName != nameof(View)
             && propertyName != nameof(IsReadOnly))
                IsDirty = true;
        }

        /// <summary>
        /// Sets a property if <see cref="IsReadOnly"/> is not true and the value is different and raises an event on the <see cref="PropertyChangedEventHandler"/>
        /// </summary>
        /// <typeparam name="T">Type of the property to set</typeparam>
        /// <param name="storage">Reference to the storage variable</param>
        /// <param name="value">New value to set</param>
        /// <param name="propertyName">Name of the property</param>
        /// <returns>True if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        protected sealed override bool SetProperty<T>(ref T storage, T value, [CallerMemberName] string propertyName = "")
        {
            if (IsReadOnly && propertyName != "IsReadOnly")
                return false;
            return base.SetProperty(ref storage, value, propertyName);
        }
    }
}
