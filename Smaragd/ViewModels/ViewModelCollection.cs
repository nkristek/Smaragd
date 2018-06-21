using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

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
        private readonly Dictionary<INotifyCollectionChanged, Tuple<string, IList<TViewModel>>> _knownCollections = new Dictionary<INotifyCollectionChanged, Tuple<string, IList<TViewModel>>>();

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

            _knownCollections[collection] = new Tuple<string, IList<TViewModel>>(collectionPropertyName, new List<TViewModel>(collection));

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
            foreach (var collectionTuple in _knownCollections.Values)
                if (collectionTuple.Item2.Contains(childViewModel))
                    yield return collectionTuple.Item1;
        }

        private void OnSubCollectionChanged(object source, NotifyCollectionChangedEventArgs args)
        {
            var notifyCollectionChanged = source as INotifyCollectionChanged;
            if (notifyCollectionChanged == null)
                return;

            var collection = source as IEnumerable<TViewModel>;
            if (collection == null)
                return;

            if (!_knownCollections.TryGetValue(notifyCollectionChanged, out var knownCollection))
                return;
            var knownItemsCollection = knownCollection.Item2;

            // olditems and newitems will usually be emtpy when the reset event is invoked => remember items from collection separately
            switch (args.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    if (args.NewItems != null)
                    {
                        foreach (TViewModel newItem in args.NewItems)
                        {
                            newItem.Parent = Parent;
                            _allChildren.Add(newItem);
                            knownItemsCollection.Add(newItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    if (args.OldItems != null)
                    {
                        foreach (TViewModel oldItem in args.OldItems)
                        {
                            oldItem.Parent = null;
                            _allChildren.Add(oldItem);
                            knownItemsCollection.Remove(oldItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Replace:
                    if (args.OldItems != null)
                    {
                        foreach (TViewModel oldItem in args.OldItems)
                        {
                            oldItem.Parent = null;
                            _allChildren.Add(oldItem);
                            knownItemsCollection.Remove(oldItem);
                        }
                    }

                    if (args.NewItems != null)
                    {
                        foreach (TViewModel newItem in args.NewItems)
                        {
                            newItem.Parent = Parent;
                            _allChildren.Add(newItem);
                            knownItemsCollection.Add(newItem);
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    // remove all items that where in the collection and readd them from the current collection

                    foreach (var knownItem in knownItemsCollection)
                    {
                        knownItem.Parent = null;
                        _allChildren.Remove(knownItem);
                    }
                    knownItemsCollection.Clear();

                    foreach (TViewModel item in collection)
                    {
                        item.Parent = Parent;
                        _allChildren.Add(item);
                        knownItemsCollection.Add(item);
                    }

                    break;
                case NotifyCollectionChangedAction.Move:
                    break;
            }
        }
    }
}
