using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
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
            var properties = GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance);
            var collectionProperties = properties.Where(p => typeof(INotifyCollectionChanged).IsAssignableFrom(p.PropertyType));
            foreach (var collectionProperty in collectionProperties.Where(p => !IsDirtyIgnoredProperties.Contains(p.Name)))
            {
                if (collectionProperty.GetValue(this, null) is INotifyCollectionChanged collection)
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

        private readonly IDictionary<string, ICommand> _commands = new Dictionary<string, ICommand>();

        /// <inheritdoc />
        public IReadOnlyDictionary<string, ICommand> Commands => new ReadOnlyDictionary<string, ICommand>(_commands);

        /// <inheritdoc />
        public void AddCommand(INamedCommand command)
        {
            _commands[command.Name] = command;
        }

        /// <inheritdoc />
        public bool RemoveCommand(INamedCommand command)
        {
            return _commands.Remove(command.Name);
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

            if (IsDirtyIgnoredProperties.Contains(propertyName))
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