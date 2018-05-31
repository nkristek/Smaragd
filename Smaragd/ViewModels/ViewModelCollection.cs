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
        private readonly Dictionary<INotifyCollectionChanged, IList> _knownCollections = new Dictionary<INotifyCollectionChanged, IList>();

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
        /// Adds a <see cref="ObservableCollection{T}"/> to this <see cref="ViewModelCollection{TViewModel}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public void AddCollection<T>(ObservableCollection<T> collection) where T : TViewModel
        {
            if (_knownCollections.ContainsKey(collection))
                throw new Exception("Collection already exists in this ViewModelCollection");
            
            _knownCollections[collection] = new List<T>(collection);

            foreach (var item in collection)
            {
                item.Parent = Parent;
                _allChildren.Add(item);
            }

            collection.CollectionChanged += OnSubCollectionChanged;
        }

        /// <summary>
        /// Removes a <see cref="ObservableCollection{T}"/> from this <see cref="ViewModelCollection{TViewModel}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public void RemoveCollection<T>(ObservableCollection<T> collection) where T : TViewModel
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

        private void OnSubCollectionChanged(object source, NotifyCollectionChangedEventArgs args)
        {
            var notifyCollectionChanged = source as INotifyCollectionChanged;
            if (notifyCollectionChanged == null)
                return;

            var collection = source as ICollection;
            if (collection == null)
                return;

            if (!_knownCollections.TryGetValue(notifyCollectionChanged, out var knownItemsCollection))
                return;

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

                    foreach (TViewModel knownItem in knownItemsCollection)
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
