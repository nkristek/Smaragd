using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.ViewModels.Helpers;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// INotifyPropertyChanged implementation
    /// </summary>
    public abstract class BindableBase
        : INotifyPropertyChanged
    {
        private readonly object _lockObject = new object();

        public virtual event PropertyChangedEventHandler PropertyChanged;

        private bool _propertyChangedNotificationsSuspended;

        /// <summary>
        /// If the <see cref="PropertyChanged"/> events are temporarily suspended. Dispose the <see cref="IDisposable"/> from <see cref="SuspendPropertyChangedNotifications"/> to unsuspend.
        /// </summary>
        public bool PropertyChangedNotificationsSuspended
        {
            get
            {
                lock (_lockObject)
                {
                    return _propertyChangedNotificationsSuspended;
                }
            }

            internal set
            {
                lock (_lockObject)
                {
                    if (value == _propertyChangedNotificationsSuspended)
                        return;

                    _propertyChangedNotificationsSuspended = value;
                    OnPropertyChangedNotificationsSuspendedChanged(value);
                }
            }
        }

        internal virtual void OnPropertyChangedNotificationsSuspendedChanged(bool suspended)
        {
        }

        /// <summary>
        /// Raises an event on the <see cref="PropertyChangedEventHandler"/>
        /// </summary>
        /// <param name="propertyName">Name of the property which changed</param>
        protected void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            InternalRaisePropertyChanged(propertyName);
        }

        internal virtual bool InternalRaisePropertyChanged(string propertyName)
        {
            if (!PropertyChangedNotificationsSuspended)
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            return !PropertyChangedNotificationsSuspended;
        }

        /// <summary>
        /// Sets a property value if the value is different and raises an event on the <see cref="PropertyChangedEventHandler"/>
        /// </summary>
        /// <typeparam name="T">Type of the property to set</typeparam>
        /// <param name="storage">Reference to the storage variable</param>
        /// <param name="value">New value to set</param>
        /// <param name="propertyName">Name of the property</param>
        /// <param name="oldValue">The old value of the property</param>
        /// <returns>True if the value was different from the storage variable and the PropertyChanged event was raised</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            oldValue = storage;
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// Temporarily suspends all events on <see cref="PropertyChanged"/>. This could be used in a batch update to prevent <see cref="PropertyChanged"/> overhead.
        /// </summary>
        /// <returns><see cref="IDisposable"/> which unsuspends notifications when disposed.</returns>
        public IDisposable SuspendPropertyChangedNotifications()
        {
            return new SuspendNotificationsDisposable(this);
        }
    }
}
