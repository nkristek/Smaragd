using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc />
    public class ViewModelCollection
        : ViewModelCollection<ViewModel>
    {
        /// <inheritdoc />
        public ViewModelCollection(ObservableCollection<ViewModel> allChildren, ViewModel parent) : base(allChildren, parent) { }
    }

    /// <inheritdoc />
    /// <summary>
    /// This is a <see cref="ReadOnlyObservableCollection{T}" /> which combines multiple <see cref="ViewModel" /> collections.
    /// </summary>
    /// <typeparam name="TViewModel">Type of items in this collection</typeparam>
    public class ViewModelCollection<TViewModel>
        : ReadOnlyObservableCollection<TViewModel> where TViewModel : ViewModel
    {
        private readonly Dictionary<INotifyCollectionChanged, string> _knownCollections = new Dictionary<INotifyCollectionChanged, string>();

        private readonly Dictionary<ViewModel, string> _childViewModelPropertyMapping = new Dictionary<ViewModel, string>();

        private readonly ObservableCollection<TViewModel> _allChildren;

        private WeakReference<ViewModel> _parent;
        
        private ViewModel Parent
        {
            get
            {
                if (_parent != null && _parent.TryGetTarget(out var viewModel))
                    return viewModel;
                return null;
            }

            set
            {
                if (Parent == value) return;
                _parent = value != null ? new WeakReference<ViewModel>(value) : null;
            }
        }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of this class.
        /// </summary>
        /// <param name="allChildren">This collection will be used and should initially be empty.</param>
        /// <param name="parent">Parent <see cref="T:NKristek.Smaragd.ViewModels.ViewModel" /> of this collection</param>
        public ViewModelCollection(ObservableCollection<TViewModel> allChildren, ViewModel parent) : base(allChildren)
        {
            _allChildren = allChildren;
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }
        
        /// <summary>
        /// Add a <see cref="ViewModel"/> to this collection and set its <see cref="Parent"/>.
        /// </summary>
        /// <param name="childViewModel"><see cref="ViewModel"/> to add</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="propagatePropertyChanged">Determines if an event on the <see cref="BindableBase.PropertyChanged"/> for this property should be raised if an propertychanged event on the childviewmodel was raised.</param>
        public void AddViewModel(TViewModel childViewModel, string propertyName, bool propagatePropertyChanged = true)
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

            HandleNewItems(Enumerable.Repeat(childViewModel, 1));
        }

        /// <summary>
        /// Remove a <see cref="ViewModel"/> from this collection and stops propagating <see cref="INotifyPropertyChanged.PropertyChanged"/> events.
        /// </summary>
        /// <param name="childViewModel"><see cref="ViewModel"/> to remove</param>
        public void RemoveViewModel(TViewModel childViewModel)
        {
            if (childViewModel == null)
                throw new ArgumentNullException(nameof(childViewModel));
            
            if (!_childViewModelPropertyMapping.Remove(childViewModel))
                return;

            childViewModel.PropertyChanged -= _ChildViewModel_PropertyChanged;

            HandleOldItems(Enumerable.Repeat(childViewModel, 1));
        }
        
        private void _ChildViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            var childViewModel = sender as ViewModel;
            if (childViewModel == null)
                return;

            if (!_childViewModelPropertyMapping.ContainsKey(childViewModel))
                return;

            var childViewModelPropertyName = _childViewModelPropertyMapping[childViewModel];
            Parent?.InternalRaisePropertyChanged(childViewModelPropertyName);
        }

        /// <summary>
        /// Adds a collection of <see cref="ViewModel"/> instances to this <see cref="ViewModelCollection{TViewModel}"/>.
        /// </summary>
        /// <typeparam name="TCollection">Type of the collection</typeparam>
        /// <param name="collection">The collection to add</param>
        /// <param name="collectionPropertyName">The name of the property in the containing <see cref="ViewModel"/>. This will be used </param>
        public void AddCollection<TCollection>(TCollection collection, string collectionPropertyName)
            where TCollection : IEnumerable<TViewModel>, INotifyCollectionChanged
        {
            if (_knownCollections.ContainsKey(collection))
                throw new Exception("Collection already exists in this ViewModelCollection");

            _knownCollections[collection] = collectionPropertyName;

            HandleNewItems(collection);

            collection.CollectionChanged += OnSubCollectionChanged;
        }

        /// <summary>
        /// Removes the collection from this <see cref="ViewModelCollection"/>
        /// </summary>
        /// <typeparam name="TCollection">Type of the collection</typeparam>
        /// <param name="collection">The collection to remove</param>
        public void RemoveCollection<TCollection>(TCollection collection)
            where TCollection : IEnumerable<TViewModel>, INotifyCollectionChanged
        {
            if (!_knownCollections.Remove(collection))
                throw new Exception("Collection does not exist in this ViewModelCollection");

            collection.CollectionChanged -= OnSubCollectionChanged;

            HandleOldItems(collection);
        }

        /// <summary>
        /// Searches the <paramref name="childViewModel"/> in all added collections and gives the property names of all collection which contain the given viewmodel
        /// </summary>
        /// <param name="childViewModel"></param>
        /// <returns>All collection names in which the item was found</returns>
        internal IEnumerable<string> GetContainingCollectionPropertyNames(TViewModel childViewModel)
        {
            foreach (var collectionKvp in _knownCollections)
                if (collectionKvp.Key is IEnumerable<TViewModel> collection && collection.Any(vm => vm == childViewModel))
                    yield return collectionKvp.Value;
        }

        /// <summary>
        /// Gets the property name of this <see cref="ViewModel"/>. It returns null when the <see cref="ViewModel"/> was added through a collection.
        /// </summary>
        /// <param name="childViewModel"><see cref="ViewModel"/> of which the property name should be retrieved</param>
        /// <returns>The property name of this <see cref="ViewModel"/> if it hasn't been added through a collection, null otherwise.</returns>
        internal string GetChildViewModelPropertyName(TViewModel childViewModel)
        {
            return _childViewModelPropertyMapping.ContainsKey(childViewModel)
                ? _childViewModelPropertyMapping[childViewModel]
                : null;
        }

        private void OnSubCollectionChanged(object source, NotifyCollectionChangedEventArgs args)
        {
            var notifyCollectionChanged = source as INotifyCollectionChanged;
            if (notifyCollectionChanged == null)
                return;

            var collection = source as IEnumerable<TViewModel>;
            if (collection == null)
                return;
            
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewItems != null)
                        HandleNewItems(args.NewItems.OfType<TViewModel>());
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (args.OldItems != null)
                        HandleOldItems(args.OldItems.OfType<TViewModel>());
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (args.OldItems != null)
                        HandleOldItems(args.OldItems.OfType<TViewModel>());
                    if (args.NewItems != null)
                        HandleNewItems(args.NewItems.OfType<TViewModel>());
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // remove all items that are not referenced by a registered collection
                    HandleOldItems(_allChildren.ToList());
                    HandleNewItems(collection);
                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
            }
        }

        private bool AnyKnownCollectionContainsItem(TViewModel item)
        {
            return _knownCollections.Keys.Any(c => c is IEnumerable<TViewModel> enumerable && enumerable.Any(i => i == item));
        }

        private void HandleOldItems(IEnumerable<TViewModel> items)
        {
            foreach (var oldItem in items.Where(i => !AnyKnownCollectionContainsItem(i) && !_childViewModelPropertyMapping.ContainsKey(i)))
            {
                oldItem.Parent = null;
                _allChildren.Remove(oldItem);
            }
        }

        private void HandleNewItems(IEnumerable<TViewModel> items)
        {
            foreach (var newItem in items.Where(i => !_allChildren.Contains(i)))
            {
                newItem.Parent = Parent;
                _allChildren.Add(newItem);
            }
        }
    }
}
