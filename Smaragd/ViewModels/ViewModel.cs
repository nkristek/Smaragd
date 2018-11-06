using System;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    /// <summary>
    /// ViewModel implementation, it supports the <see cref="IsDirtyIgnoredAttribute" /> above properties to prevent setting <see cref="IsDirty" /> for the property in question
    /// </summary>
    public abstract class ViewModel
        : ComputedBindableBase
    {
        /// <inheritdoc />
        protected ViewModel()
        {
            // set IsDirty when any collection changes
            var collectionProperties = GetType().GetProperties().Where(p => p.GetMethod.IsPublic && typeof(INotifyCollectionChanged).IsAssignableFrom(p.PropertyType));
            foreach (var property in collectionProperties)
            {
                if (!PropertyNameHasAttribute<IsDirtyIgnoredAttribute>(property.Name) && property.GetValue(this, null) is INotifyCollectionChanged collection)
                    collection.CollectionChanged += OnChildCollectionChanged;
            }
        }

        private bool _isDirty;

        /// <summary>
        /// Indicates if a property changed on the <see cref="ViewModel"/> and the change is not persisted
        /// </summary>
        [IsDirtyIgnored]
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value, out _);
        }

        private WeakReference<ViewModel> _parent;

        /// <summary>
        /// The parent of this <see cref="ViewModel"/>.
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

        /// <inheritdoc />
        /// <summary>
        /// Sets a property if <see cref="IsReadOnly" /> is not true and the value is different and raises an event on the <see cref="PropertyChangedEventHandler" />
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
            if (!propertyWasChanged)
                return false;

            if (PropertyNameHasAttribute<IsDirtyIgnoredAttribute>(propertyName))
                return true;

            IsDirty = true;

            if (oldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= OnChildCollectionChanged;

            if (storage is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += OnChildCollectionChanged;

            return true;
        }
        
        private void OnChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }
    }
}
