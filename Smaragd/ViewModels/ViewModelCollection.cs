using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace NKristek.Smaragd.ViewModels
{
    public class ViewModelCollection
        : ViewModelCollection<ViewModel>
    {
        public ViewModelCollection(ObservableCollection<ViewModel> allChildren, ViewModel parent) : base(allChildren, parent) { }
    }

    public class ViewModelCollection<TViewModel>
        : ReadOnlyObservableCollection<TViewModel> where TViewModel : ViewModel
    {
        private readonly Dictionary<INotifyCollectionChanged, string> _knownCollections = new Dictionary<INotifyCollectionChanged, string>();
        
        private readonly ObservableCollection<TViewModel> _allChildren;

        private WeakReference<ViewModel> _parent;

        /// <summary>
        /// The <see cref="ValidatingViewModel"/> of which the validation should be suspended.
        /// </summary>
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

        public ViewModelCollection(ObservableCollection<TViewModel> allChildren, ViewModel parent) : base(allChildren)
        {
            _allChildren = allChildren;
            Parent = parent;
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

            foreach (var item in collection)
            {
                item.Parent = Parent;
                _allChildren.Add(item);
            }

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

            foreach (var item in collection)
            {
                item.Parent = null;
                _allChildren.Remove(item);
            }
        }

        /// <summary>
        /// Searches the <paramref name="childViewModel"/> in all added collections and gives the property names of all collection which contain the given viewmodel
        /// </summary>
        /// <param name="childViewModel"></param>
        /// <returns>All collection names in which the item was found</returns>
        internal IEnumerable<string> GetContainingCollectionPropertyNames(TViewModel childViewModel)
        {
            foreach (var collectionKvp in _knownCollections)
            {
                if (collectionKvp.Key is IEnumerable<TViewModel> collection && collection.Any(vm => vm == childViewModel))
                    yield return collectionKvp.Value;
            }
        }

        private void OnSubCollectionChanged(object source, NotifyCollectionChangedEventArgs args)
        {
            var notifyCollectionChanged = source as INotifyCollectionChanged;
            if (notifyCollectionChanged == null)
                return;

            var collection = source as IEnumerable<TViewModel>;
            if (collection == null)
                return;
            
            // olditems and newitems will usually be emtpy when the reset event is invoked => remember items from collection separately
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
            foreach (var oldItem in items.Where(i => !AnyKnownCollectionContainsItem(i)))
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
