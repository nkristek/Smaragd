using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Helpers;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IViewModel" />
    public abstract class ViewModel
        : ValidatingBindable, IViewModel
    {
        private readonly INotificationCache _notificationCache = new NotificationCache();

        private readonly HashSet<string> _isDirtyIgnoredProperties = new HashSet<string>();

        private readonly HashSet<string> _isReadOnlyIgnoredProperties = new HashSet<string>();

        /// <inheritdoc />
        protected ViewModel()
        {
            InitAttributes();

            PropertyChanged += OnPropertyChanged;

            var collections = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => !_isDirtyIgnoredProperties.Contains(p.Name))
                .Where(p => typeof(INotifyCollectionChanged).IsAssignableFrom(p.PropertyType))
                .Select(p => p.GetValue(this, null))
                .OfType<INotifyCollectionChanged>();
            foreach (var collection in collections)
                collection.CollectionChanged += OnChildCollectionChanged;
        }

        private void InitAttributes()
        {
            var inheritPropertySource = new Dictionary<string, bool>();
            var inheritIsDirtyIgnored = new Dictionary<string, bool>();
            var inheritIsReadOnlyIgnored = new Dictionary<string, bool>();

            var currentType = GetType();
            while (currentType != null)
            {
                var properties = currentType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly);
                foreach (var property in properties)
                {
                    var attributes = property.GetCustomAttributes(false);
                    var propertyName = property.Name;
                    if (property.GetIndexParameters().Any())
                        propertyName += "[]";

                    // PropertySourceAttribute

                    if (!inheritPropertySource.ContainsKey(propertyName) || inheritPropertySource[propertyName])
                    {
                        var propertySourceAttribute = attributes.OfType<PropertySourceAttribute>().SingleOrDefault();
                        if (propertySourceAttribute != null)
                        {
                            inheritPropertySource[propertyName] = propertySourceAttribute.InheritAttributes;

                            if (propertySourceAttribute.PropertySources != null)
                                foreach (var propertySource in propertySourceAttribute.PropertySources)
                                    _notificationCache.AddPropertyNameToNotify(propertySource, propertyName);
                        }
                        else
                        {
                            inheritPropertySource[propertyName] = false;
                        }
                    }

                    // IsDirtyIgnoredAttribute

                    if (!inheritIsDirtyIgnored.ContainsKey(propertyName) || inheritIsDirtyIgnored[propertyName])
                    {
                        var isDirtyIgnoredAttribute = attributes.OfType<IsDirtyIgnoredAttribute>().SingleOrDefault();
                        if (isDirtyIgnoredAttribute != null)
                        {
                            if (isDirtyIgnoredAttribute.InheritAttributes)
                                inheritIsDirtyIgnored[propertyName] = true;
                            else
                                _isDirtyIgnoredProperties.Add(propertyName);
                        }
                        else
                        {
                            inheritIsDirtyIgnored[propertyName] = false;
                        }
                    }

                    // IsReadOnlyIgnoredAttribute

                    if (!inheritIsReadOnlyIgnored.ContainsKey(propertyName) || inheritIsReadOnlyIgnored[propertyName])
                    {
                        var isReadOnlyIgnoredAttribute = attributes.OfType<IsReadOnlyIgnoredAttribute>().SingleOrDefault();
                        if (isReadOnlyIgnoredAttribute != null)
                        {
                            if (isReadOnlyIgnoredAttribute.InheritAttributes)
                                inheritIsReadOnlyIgnored[propertyName] = true;
                            else
                                _isReadOnlyIgnoredProperties.Add(propertyName);
                        }
                        else
                        {
                            inheritIsReadOnlyIgnored[propertyName] = false;
                        }
                    }
                }

                currentType = currentType.BaseType;
            }
        }

        private void OnPropertyChanged(object? sender, PropertyChangedEventArgs? e)
        {
            if (e == null
            ||  String.IsNullOrEmpty(e.PropertyName)
            || !_isDirtyIgnoredProperties.Contains(e.PropertyName))
                IsDirty = true;
        }

        private void OnChildCollectionChanged(object? sender, NotifyCollectionChangedEventArgs? e)
        {
            IsDirty = true;
        }

        /// <inheritdoc />
        [IsDirtyIgnored]
        public override bool HasErrors => base.HasErrors;

        /// <inheritdoc />
        [IsDirtyIgnored]
        [PropertySource(nameof(HasErrors))]
        public virtual bool IsValid => !HasErrors;

        private bool _isDirty;

        /// <inheritdoc />
        [IsDirtyIgnored]
        [IsReadOnlyIgnored]
        public virtual bool IsDirty
        {
            get => _isDirty;
            set => SetProperty(ref _isDirty, value);
        }

        private WeakReference<IViewModel>? _parent;

        /// <inheritdoc />
        [IsDirtyIgnored]
        [IsReadOnlyIgnored]
        public virtual IViewModel? Parent
        {
            get => _parent?.TargetOrDefault();
            set => SetProperty(ref _parent, value);
        }

        private bool _isReadOnly;

        /// <inheritdoc />
        [IsDirtyIgnored]
        [IsReadOnlyIgnored]
        public virtual bool IsReadOnly
        {
            get => _isReadOnly;
            set => SetProperty(ref _isReadOnly, value);
        }

        private bool _isUpdating;

        /// <inheritdoc />
        [IsDirtyIgnored]
        [IsReadOnlyIgnored]
        public virtual bool IsUpdating
        {
            get => _isUpdating;
            set => SetProperty(ref _isUpdating, value);
        }

        /// <inheritdoc />
        /// <remarks>
        /// It also raises events for each property name which gets notified by the given <paramref name="propertyName"/>.
        /// </remarks>
        protected override void NotifyPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            base.NotifyPropertyChanging(propertyName);
            if (String.IsNullOrEmpty(propertyName))
                return;

            foreach (var propertyNameToNotify in _notificationCache.GetPropertyNamesToNotify(propertyName!))
                base.NotifyPropertyChanging(propertyNameToNotify);
        }

        /// <inheritdoc />
        /// <remarks>
        /// It also raises events for each property name which gets notified by the given <paramref name="propertyName"/>.
        /// </remarks>
        protected override void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            base.NotifyPropertyChanged(propertyName);
            if (String.IsNullOrEmpty(propertyName))
                return;

            foreach (var propertyNameToNotify in _notificationCache.GetPropertyNamesToNotify(propertyName!))
                base.NotifyPropertyChanged(propertyNameToNotify);
        }

        /// <inheritdoc />
        /// <remarks>
        /// Set the property value only if <see cref="IsReadOnly" /> is <see langword="false"/>.
        /// </remarks>
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, IEqualityComparer<T>? comparer = null, [CallerMemberName] string? propertyName = null)
        {
            oldValue = storage;
            if (IsReadOnly && (String.IsNullOrEmpty(propertyName) || !_isReadOnlyIgnoredProperties.Contains(propertyName!)))
                return false;

            if (!base.SetProperty(ref storage, value, out oldValue, comparer, propertyName))
                return false;

            if (!String.IsNullOrEmpty(propertyName) && _isDirtyIgnoredProperties.Contains(propertyName!))
                return true;

            if (oldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= OnChildCollectionChanged;

            if (value is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += OnChildCollectionChanged;

            return true;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Set the property value only if <see cref="IsReadOnly" /> is <see langword="false"/>.
        /// </remarks>
        protected override bool SetProperty<T>(ref WeakReference<T>? storage, T? value, out T? oldValue, IEqualityComparer<T?>? comparer = null, [CallerMemberName] string? propertyName = null) 
            where T : class
        {
            oldValue = storage?.TargetOrDefault();
            if (IsReadOnly && (String.IsNullOrEmpty(propertyName) || !_isReadOnlyIgnoredProperties.Contains(propertyName!)))
                return false;

            if (!base.SetProperty(ref storage, value, out oldValue, comparer, propertyName))
                return false;

            if (!String.IsNullOrEmpty(propertyName) && _isDirtyIgnoredProperties.Contains(propertyName!))
                return true;

            if (oldValue is INotifyCollectionChanged oldCollection)
                oldCollection.CollectionChanged -= OnChildCollectionChanged;

            if (value is INotifyCollectionChanged newCollection)
                newCollection.CollectionChanged += OnChildCollectionChanged;

            return true;
        }
    }
}