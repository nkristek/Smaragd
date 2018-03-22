using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
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

        private readonly HashSet<string> _ViewModelProperties = new HashSet<string>
        {
            nameof(IsDirty),
            nameof(Parent),
            nameof(View),
            nameof(IsReadOnly)
        };
        
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

        /// <summary>
        /// This will set the <see cref="IsDirty"/> property to true is a property was changed, 
        /// It ignores <see cref="IsDirty"/>, <see cref="Parent"/>, <see cref="View"/> and <see cref="IsReadOnly"/>.
        /// Override if you want different behaviour.
        /// </summary>
        /// <param name="propertyName">Name of the property which was changed</param>
        protected override void OnPropertyChanged(string propertyName)
        {
            if (!_ViewModelProperties.Contains(propertyName))
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
                throw new ArgumentNullException("childViewModel");

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

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
                throw new ArgumentNullException("childViewModel");

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            if (!_ChildViewModelPropertyMapping.ContainsKey(childViewModel))
                return;

            _ChildViewModelPropertyMapping.Remove(childViewModel);
            childViewModel.PropertyChanged -= _ChildViewModel_PropertyChanged;
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

            if (!_ChildViewModelPropertyMapping.ContainsKey(childViewModel))
                return;

            if (!_ViewModelProperties.Contains(e.PropertyName))
            {
                var childViewModelPropertyName = _ChildViewModelPropertyMapping[childViewModel];
                RaisePropertyChanged(childViewModelPropertyName);
            }
        }


        private readonly Dictionary<ObservableCollection<ViewModel>, string> _ChildViewModelCollectionPropertyMapping = new Dictionary<ObservableCollection<ViewModel>, string>();

        /// <summary>
        /// Register an <see cref="ObservableCollection{ViewModel}"/> so property changes of items of the collection raise a PropertyChanged event for the collection
        /// </summary>
        /// <param name="childViewModelCollection"></param>
        /// <param name="propertyName"></param>
        protected void RegisterChildViewModelCollection(ObservableCollection<ViewModel> childViewModelCollection, [CallerMemberName] string propertyName = "")
        {
            if (childViewModelCollection == null)
                throw new ArgumentNullException("childViewModelCollection");

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            if (_ChildViewModelCollectionPropertyMapping.ContainsKey(childViewModelCollection))
                return;

            _ChildViewModelCollectionPropertyMapping.Add(childViewModelCollection, propertyName);
            childViewModelCollection.CollectionChanged += _ChildViewModelCollection_CollectionChanged;

            foreach (var childViewModel in childViewModelCollection)
                RegisterChildViewModel(childViewModel, propertyName);
        }

        /// <summary>
        /// Unregisters an <see cref="ObservableCollection{ViewModel}"/> so property changes of items of the collection no longer raise a PropertyChanged event for the collection
        /// </summary>
        /// <param name="childViewModelCollection"></param>
        /// <param name="propertyName"></param>
        protected void UnregisterChildViewModelCollection(ObservableCollection<ViewModel> childViewModelCollection, [CallerMemberName] string propertyName = "")
        {
            if (childViewModelCollection == null)
                throw new ArgumentNullException("childViewModelCollection");

            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException("propertyName");

            if (!_ChildViewModelCollectionPropertyMapping.ContainsKey(childViewModelCollection))
                return;

            _ChildViewModelCollectionPropertyMapping.Remove(childViewModelCollection);
            childViewModelCollection.CollectionChanged -= _ChildViewModelCollection_CollectionChanged;

            foreach (var childViewModel in childViewModelCollection)
                UnregisterChildViewModel(childViewModel, propertyName);
        }

        /// <summary>
        /// This method iterates over changes in the collection and registers/unregisters child <see cref="ViewModel"/> instances accordingly
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void _ChildViewModelCollection_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            var childViewModelCollection = sender as ObservableCollection<ViewModel>;
            if (childViewModelCollection == null)
                return;

            if (!_ChildViewModelCollectionPropertyMapping.ContainsKey(childViewModelCollection))
                return;

            var childViewModelCollectionPropertyName = _ChildViewModelCollectionPropertyMapping[childViewModelCollection];
            OnPropertyChanged(childViewModelCollectionPropertyName);

            if (e.NewItems != null)
                foreach (var newItem in e.NewItems)
                    if (newItem is ViewModel childViewModel)
                        RegisterChildViewModel(childViewModel, nameof(childViewModelCollectionPropertyName));

            if (e.OldItems != null)
                foreach (var oldItem in e.OldItems)
                    if (oldItem is ViewModel childViewModel)
                        UnregisterChildViewModel(childViewModel, nameof(childViewModelCollectionPropertyName));
        }

        /// <summary>
        /// Gets the first parent of the requested <typeparamref name="T"/>
        /// </summary>
        /// <typeparam name="T">Type of the requested parent</typeparam>
        /// <returns>The first parent of the requested type</returns>
        public T FirstParentOfType<T>() where T : ViewModel
        {
            var parent = Parent;
            return parent as T ?? parent?.FirstParentOfType<T>();
        }
    }
}
