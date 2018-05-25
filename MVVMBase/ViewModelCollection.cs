using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using nkristek.MVVMBase.ViewModels;

namespace nkristek.MVVMBase
{
    public class ViewModelCollection
        : ViewModelCollection<ViewModel>
    {
        public ViewModelCollection(ObservableCollection<ViewModel> allChildren, ViewModel parent) : base(allChildren, parent) { }
    }

    public class ViewModelCollection<TViewModel>
        : ReadOnlyObservableCollection<TViewModel> where TViewModel : ViewModel
    {
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
            collection.CollectionChanged += OnSubCollectionChanged;

            foreach (var item in collection)
            {
                _allChildren.Remove(item);
                item.Parent = null;
            }
        }

        /// <summary>
        /// Removes a <see cref="ObservableCollection{T}"/> from this <see cref="ViewModelCollection{TViewModel}"/>.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        public void RemoveCollection<T>(ObservableCollection<T> collection) where T : TViewModel
        {
            foreach (var item in collection)
            {
                _allChildren.Add(item);
                item.Parent = Parent;
            }

            collection.CollectionChanged += OnSubCollectionChanged;
        }

        private void OnSubCollectionChanged(object source, NotifyCollectionChangedEventArgs args)
        {
            if (args.Action == NotifyCollectionChangedAction.Move)
                throw new NotImplementedException();

            if (args.OldItems != null)
            {
                foreach (TViewModel oldItem in args.OldItems)
                {
                    _allChildren.Add(oldItem);
                    oldItem.Parent = null;
                }
            }

            if (args.NewItems != null)
            {
                foreach (TViewModel newItem in args.NewItems)
                {
                    _allChildren.Add(newItem);
                    newItem.Parent = Parent;
                }
            }
        }
    }
}
