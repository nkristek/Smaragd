using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IViewModel" />
    public abstract class ViewModel
        : ComputedBindable, IViewModel
    {
        /// <inheritdoc />
        protected ViewModel()
        {
            var collectionProperties = GetType().GetProperties().Where(p => p.GetMethod.IsPublic);
            foreach (var property in collectionProperties)
            {
                if (!PropertyNameHasAttribute<IsDirtyIgnoredAttribute>(property.Name)
                    && typeof(INotifyCollectionChanged).IsAssignableFrom(property.PropertyType)
                    && property.GetValue(this, null) is INotifyCollectionChanged collection)
                    collection.CollectionChanged += OnChildCollectionChanged;
            }
        }

        private bool _isDirty;

        /// <inheritdoc />
        [IsDirtyIgnored]
        public bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value, out _);
        }

        private WeakReference<IViewModel> _parent;

        /// <inheritdoc />
        [IsDirtyIgnored]
        public IViewModel Parent
        {
            get => _parent != null && _parent.TryGetTarget(out var parent) ? parent : null;
            set
            {
                if (Parent == value) return;
                _parent = value != null ? new WeakReference<IViewModel>(value) : null;
                RaisePropertyChanged();
            }
        }

        private bool _isReadOnly;

        /// <inheritdoc />
        [IsDirtyIgnored]
        public virtual bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value, out _);
        }

        private Dictionary<string, ICommand> _commands;

        /// <inheritdoc />
        public virtual Dictionary<string, ICommand> Commands => _commands ?? (_commands = new Dictionary<string, ICommand>());

        /// <inheritdoc />
        protected override void RaisePropertyChanged(string propertyName, IEnumerable<string> additionalPropertyNames)
        {
            var additionalPropertyNamesList = additionalPropertyNames.ToList();
            base.RaisePropertyChanged(propertyName, additionalPropertyNamesList);

            var propertyNamesToNotify = new List<string> {propertyName};
            propertyNamesToNotify.AddRange(additionalPropertyNamesList);

            foreach (var command in Commands.Select(c => c.Value).OfType<IRaiseCanExecuteChanged>())
                if (command.ShouldRaiseCanExecuteChanged(propertyNamesToNotify))
                    command.RaiseCanExecuteChanged();
        }

        /// <inheritdoc />
        /// <remarks>
        /// Set the property value only if <see cref="IsReadOnly" /> is <c>false</c>.
        /// </remarks>
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