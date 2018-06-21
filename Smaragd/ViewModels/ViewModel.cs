using System;
using System.Collections.ObjectModel;
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
            foreach (var collectionProperty in GetType().GetProperties().Where(p => p.GetMethod.IsPublic && typeof(INotifyCollectionChanged).IsAssignableFrom(p.PropertyType)))
            {
                if (PropertyNameHasAttribute<IsDirtyIgnoredAttribute>(collectionProperty.Name))
                    continue;

                if (collectionProperty.GetValue(this, null) is INotifyCollectionChanged collection)
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
            set
            {
                if (SetProperty(ref _isDirty, value, out _))
                {
                    if (value)
                    {
                        var parent = Parent;
                        if (parent != null && !parent.IsDirty)
                            parent.SetIsDirty(this);
                    }
                    else
                    {
                        foreach (var child in Children.Where(c => c.IsDirty))
                            child.IsDirty = false;
                    }
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

                if (value == null)
                    return;

                if (IsDirty)
                    value.SetIsDirty(this);

                if (value.IsReadOnly)
                    IsReadOnly = true;
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
            set
            {
                if (SetProperty(ref _isReadOnly, value, out _))
                {
                    foreach (var child in Children)
                        child.IsReadOnly = value;
                }
            }
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
            if (propertyWasChanged && !PropertyNameHasAttribute<IsDirtyIgnoredAttribute>(propertyName))
            {
                IsDirty = true;
                
                if (oldValue is INotifyCollectionChanged oldCollection)
                    oldCollection.CollectionChanged -= OnChildCollectionChanged;

                if (storage is INotifyCollectionChanged newCollection)
                    newCollection.CollectionChanged += OnChildCollectionChanged;
            }
            return propertyWasChanged;
        }

        private void OnChildCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            IsDirty = true;
        }

        /// <summary>
        /// Sets <see cref="IsDirty"/> to <c>True</c> if no <see cref="IsDirtyIgnoredAttribute"/> is set on the property of the given <see cref="ViewModel"/> or on the property of the collection which contains the given <see cref="ViewModel"/>
        /// </summary>
        /// <param name="childViewModel"><see cref="ViewModel"/> which propagates the change of <see cref="IsDirty"/></param>
        internal void SetIsDirty(ViewModel childViewModel)
        {
            if (IsDirty)
                return;

            var childViewModelPropertyName = Children.GetChildViewModelPropertyName(childViewModel);
            if (String.IsNullOrEmpty(childViewModelPropertyName))
            {
                // childViewModel is no single property, look in collections instead
                foreach (var collectionPropertyName in Children.GetContainingCollectionPropertyNames(childViewModel))
                {
                    if (!PropertyNameHasAttribute<IsDirtyIgnoredAttribute>(collectionPropertyName))
                    {
                        IsDirty = true;
                        return;
                    }
                }
            }
            else
            {
                if (!PropertyNameHasAttribute<IsDirtyIgnoredAttribute>(childViewModelPropertyName))
                    IsDirty = true;
            }
        }

        private readonly ObservableCollection<ViewModel> _allChildren = new ObservableCollection<ViewModel>();

        private ViewModelCollection _children;

        /// <summary>
        /// All children of this <see cref="ViewModel"/>
        /// </summary>
        [IsDirtyIgnored]
        public ViewModelCollection Children => _children ?? (_children = new ViewModelCollection(_allChildren, this));
    }
}
