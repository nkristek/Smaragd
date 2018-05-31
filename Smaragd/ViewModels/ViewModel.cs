using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.ViewModels.Helpers;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// ViewModel implementation, it supports the <see cref="IsDirtyIgnoredAttribute"/> above properties to prevent setting <see cref="IsDirty"/> for the property in question
    /// </summary>
    public abstract class ViewModel
        : ComputedBindableBase
    {
        protected ViewModel()
        {
            // set IsDirty when a collection changes
            foreach (var collectionProperty in GetType().GetProperties().Where(p => p.GetMethod.IsPublic && typeof(INotifyCollectionChanged).IsAssignableFrom(p.PropertyType)))
            {
                if (CachedAttributes.TryGetValue(collectionProperty.Name, out var collectionAttributes) && collectionAttributes.Item2.Any(a => a is IsDirtyIgnoredAttribute))
                    continue;

                if (collectionProperty.GetValue(this, null) is INotifyCollectionChanged collection)
                    collection.CollectionChanged += OnCollectionChanged;
            }
        }

        internal override void OnPropertyChangedNotificationsSuspendedChanged(bool suspended)
        {
            foreach (var child in Children)
                child.PropertyChangedNotificationsSuspended = suspended;
        }

        private bool _isDirty;

        /// <summary>
        /// Indicates if a property changed on the <see cref="ViewModel"/> and the change is not persisted
        /// </summary>
        [IsDirtyIgnored]
        public bool IsDirty
        {
            get => _isDirty;
            set
            {
                if (SetProperty(ref _isDirty, value, out _))
                {
                    if (value)
                        if (Parent != null)
                            Parent.IsDirty = true;
                    else
                        foreach (var child in Children)
                            child.IsDirty = false;
                }
            }
        }

        private WeakReference<ViewModel> _parent;

        /// <summary>
        /// The parent of this <see cref="ViewModel"/>
        /// </summary>
        [IsDirtyIgnored]
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
        [IsDirtyIgnored]
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value, out _);
        }

        /// <summary>
        /// Sets a property if <see cref="IsReadOnly"/> is not true and the value is different and raises an event on the <see cref="PropertyChangedEventHandler"/>
        /// </summary>
        /// <typeparam name="T">Type of the property to set</typeparam>
        /// <param name="storage">Reference to the storage variable</param>
        /// <param name="value">New value to set</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="oldValue">The old value of the property</param>
        /// <returns>True if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            oldValue = storage;
            if (IsReadOnly && propertyName != nameof(IsReadOnly))
                return false;

            var propertyWasChanged = base.SetProperty(ref storage, value, out oldValue, propertyName);
            if (propertyWasChanged)
            {
                var cachedAttributesOfProperty = CachedAttributes.FirstOrDefault(ca => ca.Key == propertyName);
                if (cachedAttributesOfProperty.IsDefault() || !cachedAttributesOfProperty.Value.Item2.Any(a => a is IsDirtyIgnoredAttribute))
                {
                    IsDirty = true;

                    // set IsDirty when a collection changes
                    if (oldValue is INotifyCollectionChanged oldCollection)
                        oldCollection.CollectionChanged -= OnCollectionChanged;
                    if (storage is INotifyCollectionChanged newCollection)
                        newCollection.CollectionChanged += OnCollectionChanged;
                }
            }
            return propertyWasChanged;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action != NotifyCollectionChangedAction.Move)
                IsDirty = true;
        }
        
        private readonly Dictionary<ViewModel, string> _childViewModelPropertyMapping = new Dictionary<ViewModel, string>();

        /// <summary>
        /// Add a <see cref="ViewModel"/> to the <see cref="Children"/> collection and set its <see cref="Parent"/>.
        /// </summary>
        /// <param name="childViewModel"></param>
        /// <param name="propagatePropertyChanged">Determines if an event on the <see cref="BindableBase.PropertyChanged"/> for this property should be raised if an propertychanged event on the childviewmodel was raised.</param>
        /// <param name="propertyName"></param>
        protected void AddChildViewModel(ViewModel childViewModel, bool propagatePropertyChanged = true, [CallerMemberName] string propertyName = "")
        {
            if (childViewModel == null)
                throw new ArgumentNullException(nameof(childViewModel));

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (_childViewModelPropertyMapping.ContainsKey(childViewModel))
                return;

            _childViewModelPropertyMapping.Add(childViewModel, propertyName);

            if (propagatePropertyChanged)
                childViewModel.PropertyChanged += _ChildViewModel_PropertyChanged;

            childViewModel.Parent = this;
            _allChildren.Add(childViewModel);
        }

        /// <summary>
        /// Remove a <see cref="ViewModel"/> from the <see cref="Children"/> collection and from the <see cref="BindableBase.PropertyChanged"/> event of the child.
        /// </summary>
        /// <param name="childViewModel"></param>
        /// <param name="propertyName"></param>
        protected void RemoveChildViewModel(ViewModel childViewModel, [CallerMemberName] string propertyName = "")
        {
            if (childViewModel == null)
                throw new ArgumentNullException(nameof(childViewModel));

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (!_childViewModelPropertyMapping.ContainsKey(childViewModel))
                return;

            _childViewModelPropertyMapping.Remove(childViewModel);
            childViewModel.PropertyChanged -= _ChildViewModel_PropertyChanged;

            childViewModel.Parent = null;
            _allChildren.Remove(childViewModel);
        }

        /// <summary>
        /// Raise a PropertyChanged event for the child <see cref="ViewModel"/> if a property changed on it
        /// </summary>
        /// <param name="sender">The child <see cref="ViewModel"/></param>
        /// <param name="e">EventArgs</param>
        private void _ChildViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var childViewModel = sender as ViewModel;
            if (childViewModel == null)
                return;
            
            if (childViewModel.CachedAttributes.Any(ca => ca.Key == e.PropertyName && ca.Value.Item2.Any(a => a is IsDirtyIgnoredAttribute)))
                return;

            if (!_childViewModelPropertyMapping.ContainsKey(childViewModel))
                return;
            
            var childViewModelPropertyName = _childViewModelPropertyMapping[childViewModel];
            RaisePropertyChanged(childViewModelPropertyName);
        }

        private readonly ObservableCollection<ViewModel> _allChildren = new ObservableCollection<ViewModel>();

        private ViewModelCollection _children;

        /// <summary>
        /// All children of this <see cref="ViewModel"/>
        /// </summary>
        public ViewModelCollection Children => _children ?? (_children = new ViewModelCollection(_allChildren, this));
    }
}
