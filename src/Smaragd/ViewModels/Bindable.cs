using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="INotifyPropertyChanged" />
    /// <summary>
    /// This class provides an implementation of <see cref="INotifyPropertyChanging"/>, <see cref="IRaisePropertyChanging"/>, <see cref="INotifyPropertyChanged"/>, <see cref="IRaisePropertyChanged"/> and a method to set the value of a property and automatically raise an event on <see cref="PropertyChanged"/> if the value changed.
    /// </summary>
    public abstract class Bindable
        : IRaisePropertyChanging, IRaisePropertyChanged
    {
        #region IRaisePropertyChanging

        /// <inheritdoc />
        public virtual event PropertyChangingEventHandler PropertyChanging;

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If <paramref name="propertyName" /> is null or whitespace.</exception>
        public virtual void RaisePropertyChanging([CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var argument = new PropertyChangingEventArgs(propertyName);
            PropertyChanging?.Invoke(this, argument);
        }

        #endregion

        #region IRaisePropertyChanged

        /// <inheritdoc />
        public virtual event PropertyChangedEventHandler PropertyChanged;

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException">If <paramref name="propertyName" /> is null or whitespace.</exception>
        public virtual void RaisePropertyChanged([CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var argument = new PropertyChangedEventArgs(propertyName);
            PropertyChanged?.Invoke(this, argument);
        }

        #endregion

        /// <summary>
        /// <para>
        /// Set <paramref name="storage"/> to the given <paramref name="value"/>.
        /// </para>
        /// <para>
        /// If the given <paramref name="value"/> is different than the current value raise an event on <see cref="INotifyPropertyChanging.PropertyChanging"/> before the storage changes and <see cref="INotifyPropertyChanged.PropertyChanged"/> after the storage changed.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the property to set.</typeparam>
        /// <param name="storage">Reference to the storage variable.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <param name="oldValue">The old value of <paramref name="storage"/>.</param>
        /// <returns><see langword="true"/> if the value was different from the <paramref name="storage"/> variable and events on <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> were raised; otherwise, <see langword="false"/>.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrWhiteSpace(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            oldValue = storage;
            if (EqualityComparer<T>.Default.Equals(storage, value))
                return false;

            RaisePropertyChanging(propertyName);
            storage = value;
            RaisePropertyChanged(propertyName);
            return true;
        }
    }
}