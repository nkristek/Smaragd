using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// ViewModel implementation
    /// </summary>
    public abstract class ViewModel
        : ComputedBindableBase
    {
        public ViewModel()
        {
            PropertyChanged += (sender, e) =>
            {
                if (!GetIsDirtyIgnoredPropertyNames().Contains(e.PropertyName))
                    IsDirty = true;
            };
        }

        private bool _isDirty;
        /// <summary>
        /// Indicates if a property changed on the <see cref="ViewModel"/> and the change is not persisted
        /// </summary>
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (SetProperty(ref _isDirty, value) && value && Parent != null)
                    Parent.IsDirty = true;
            }
        }

        private WeakReference<ViewModel> _parent;
        /// <summary>
        /// The parent of this <see cref="ViewModel"/>
        /// </summary>
        public ViewModel Parent
        {
            get
            {
                if (_parent != null && _parent.TryGetTarget(out var parent))
                    return parent;
                return null;
            }

            set
            {
                if (Parent == value) return;
                _parent = value != null ? new WeakReference<ViewModel>(value) : null;
                RaisePropertyChanged();
            }
        }
        
        private bool _isReadOnly;
        /// <summary>
        /// Indicates if this <see cref="ViewModel"/> instance is read only and it is not possible to change a property value
        /// </summary>
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        /// <summary>
        /// Gets propertynames which are ignored by <see cref="IsDirty"/>
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetIsDirtyIgnoredPropertyNames()
        {
            yield return nameof(IsDirty);
            yield return nameof(Parent);
            yield return nameof(IsReadOnly);
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
            if (IsReadOnly && propertyName != nameof(IsReadOnly))
                return false;

            if (storage is ViewModel)
                UnregisterChildViewModel(storage as ViewModel, propertyName);

            var propertyWasChanged = base.SetProperty(ref storage, value, propertyName);

            if (storage is ViewModel)
                RegisterChildViewModel(storage as ViewModel, propertyName);

            return propertyWasChanged;
        }
        
        private readonly Dictionary<ViewModel, string> _ChildViewModelPropertyMapping = new Dictionary<ViewModel, string>();

        /// <summary>
        /// Register a child <see cref="ViewModel"/> so a PropertyChanged event is raised when a property changed on the child <see cref="ViewModel"/>
        /// </summary>
        /// <param name="childViewModel"></param>
        /// <param name="propertyName"></param>
        protected void RegisterChildViewModel(ViewModel childViewModel, [CallerMemberName] string propertyName = "")
        {
            if (childViewModel == null)
                throw new ArgumentNullException(nameof(childViewModel));

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (_ChildViewModelPropertyMapping.ContainsKey(childViewModel))
                return;

            _ChildViewModelPropertyMapping.Add(childViewModel, propertyName);
            childViewModel.PropertyChanged += _ChildViewModel_PropertyChanged;
        }

        /// <summary>
        /// Unregister a from the PropertyChanged event of the child <see cref="ViewModel"/>
        /// </summary>
        /// <param name="childViewModel"></param>
        /// <param name="propertyName"></param>
        protected void UnregisterChildViewModel(ViewModel childViewModel, [CallerMemberName] string propertyName = "")
        {
            if (childViewModel == null)
                throw new ArgumentNullException(nameof(childViewModel));

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (!_ChildViewModelPropertyMapping.ContainsKey(childViewModel))
                return;

            _ChildViewModelPropertyMapping.Remove(childViewModel);
            childViewModel.PropertyChanged -= _ChildViewModel_PropertyChanged;
        }

        /// <summary>
        /// Gets propertynames which are ignored when a childviewmodels propertychanged event fires
        /// </summary>
        /// <returns></returns>
        protected virtual IEnumerable<string> GetChildViewModelPropertyChangedIgnoredPropertyNames()
        {
            yield return nameof(IsDirty);
            yield return nameof(Parent);
            yield return nameof(IsReadOnly);
        }

        /// <summary>
        /// Raise a PropertyChanged event for the child <see cref="ViewModel"/> if a property changed on it
        /// </summary>
        /// <param name="sender">The child <see cref="ViewModel"/></param>
        /// <param name="e">EventArgs</param>
        private void _ChildViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (GetChildViewModelPropertyChangedIgnoredPropertyNames().Contains(e.PropertyName))
                return;

            var childViewModel = sender as ViewModel;
            if (childViewModel == null)
                return;

            if (!_ChildViewModelPropertyMapping.ContainsKey(childViewModel))
                return;
            
            var childViewModelPropertyName = _ChildViewModelPropertyMapping[childViewModel];
            RaisePropertyChanged(childViewModelPropertyName);
        }
    }
}
