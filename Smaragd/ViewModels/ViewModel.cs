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
    /// <inheritdoc />
    /// <remarks>
    /// <para>
    /// This class provides the following properties:
    /// </para>
    /// <para>
    /// - <see cref="IsDirty"/>: Is set to <c>true</c> if a property changes through <see cref="SetProperty{T}"/>, if the <see cref="IsDirtyIgnoredAttribute" /> is not present above the property.
    /// <see cref="IsDirty"/> will also be set to <c>true</c> if a property implements <see cref="INotifyCollectionChanged"/> and an event on <see cref="INotifyCollectionChanged.CollectionChanged"/> occurs.
    /// </para>
    /// <para>
    /// - <see cref="Parent"/>: Parent of this <see cref="ViewModel"/>, which uses a <see cref="WeakReference{T}"/> to prevent circular references.
    /// </para>
    /// <para>
    /// - <see cref="IsReadOnly"/>: If set to <c>true</c> <see cref="SetProperty{T}"/> will not change properties.
    /// </para>
    /// <para>
    /// - <see cref="Commands"/>: A <see cref="Dictionary{TKey,TValue}"/> of all commands of this <see cref="ViewModel"/>. The key is the name of the command, the value the command itself.
    /// </para>
    /// </remarks>
    public abstract class ViewModel
        : ComputedBindable
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

        /// <summary>
        /// Indicates if a property changed and is not persisted.
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
            get => _parent != null && _parent.TryGetTarget(out var parent) ? parent : null;
            set
            {
                if (Parent == value) return;
                _parent = value != null ? new WeakReference<ViewModel>(value) : null;
                RaisePropertyChanged();
            }
        }
        
        private bool _isReadOnly;

        /// <summary>
        /// Indicates if this <see cref="ViewModel"/> instance is read only and it is not possible to change a property value.
        /// </summary>
        [IsDirtyIgnored]
        public bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value, out _);
        }

        private Dictionary<string, ICommand> _commands;

        /// <summary>
        /// Commands of this <see cref="ViewModel"/>.
        /// </summary>
        public virtual Dictionary<string, ICommand> Commands => _commands ?? (_commands = new Dictionary<string, ICommand>());

        /// <inheritdoc />
        protected override void RaisePropertyChanged(string propertyName, IEnumerable<string> additionalPropertyNames)
        {
            var additionalPropertyNamesList = additionalPropertyNames.ToList();
            base.RaisePropertyChanged(propertyName, additionalPropertyNamesList);

            var propertyNamesToNotify = new List<string> { propertyName };
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
