using NKristek.Smaragd.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IBindable" />
    /// <remarks>
    /// This class includes methods to set the value of a property and automatically raise events on the appropriate event handlers.
    /// </remarks>
    public abstract class Bindable
        : IBindable
    {
        /// <inheritdoc />
        public event PropertyChangingEventHandler PropertyChanging;

        /// <summary>
        /// Raise an event on <see cref="INotifyPropertyChanging.PropertyChanging"/> to indicate that a property value is changing.
        /// </summary>
        /// <param name="propertyName">Name of the changing property value.</param>
        protected virtual void NotifyPropertyChanging([CallerMemberName] string? propertyName = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(propertyName));
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Raise an event on <see cref="INotifyPropertyChanged.PropertyChanged"/> to indicate that a property value changed.
        /// </summary>
        /// <param name="propertyName">Name of the changed property value.</param>
        protected virtual void NotifyPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        
        /// <summary>
        /// <para>
        /// Set <paramref name="storage"/> to the given <paramref name="value"/>.
        /// </para>
        /// <para>
        /// If the given <paramref name="value"/> is different than the current value,
        /// it raises an event on <see cref="INotifyPropertyChanging.PropertyChanging"/> before the storage changes and <see cref="INotifyPropertyChanged.PropertyChanged"/> after the storage was changed.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="storage">Reference to the storage field.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="oldValue">The old value of <paramref name="storage"/>.</param>
        /// <param name="comparer">An optional comparer to compare the value of <paramref name="storage"/> and <paramref name="value"/>. If <see langword="null"/> is passed, the default comparer will be used.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see langword="true"/> if the value was different from the <paramref name="storage"/> variable and events on <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> were raised; otherwise, <see langword="false"/>.</returns>
        protected virtual bool SetProperty<T>(ref T storage, T value, out T oldValue, IEqualityComparer<T>? comparer = null, [CallerMemberName] string? propertyName = null)
        {
            oldValue = storage;
            if ((comparer ?? EqualityComparer<T>.Default).Equals(storage, value))
                return false;

            NotifyPropertyChanging(propertyName);
            storage = value;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// <para>
        /// Set <paramref name="storage"/> to the given <paramref name="value"/>.
        /// </para>
        /// <para>
        /// If the given <paramref name="value"/> is different than the current value,
        /// it raises an event on <see cref="INotifyPropertyChanging.PropertyChanging"/> before the storage changes and <see cref="INotifyPropertyChanged.PropertyChanged"/> after the storage was changed.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="storage">Reference to the storage field.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="comparer">An optional comparer to compare the value of <paramref name="storage"/> and <paramref name="value"/>. If <see langword="null"/> is passed, the default comparer will be used.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see langword="true"/> if the value was different from the <paramref name="storage"/> variable and events on <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> were raised; otherwise, <see langword="false"/>.</returns>
        protected bool SetProperty<T>(ref T storage, T value, IEqualityComparer<T>? comparer = null, [CallerMemberName] string? propertyName = null)
        {
            return SetProperty(ref storage, value, out _, comparer, propertyName);
        }

        /// <summary>
        /// <para>
        /// Set <paramref name="storage"/> to a <see cref="WeakReference{T}"/> of the given <paramref name="value"/>.
        /// </para>
        /// <para>
        /// If the given <paramref name="value"/> is different than the current value,
        /// it raises an event on <see cref="INotifyPropertyChanging.PropertyChanging"/> before the storage changes and <see cref="INotifyPropertyChanged.PropertyChanged"/> after the storage was changed.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="storage">Reference to the storage field containing the <see cref="WeakReference{T}"/>.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="oldValue">The old value of <paramref name="storage"/>.</param>
        /// <param name="comparer">An optional comparer to compare the value of <paramref name="storage"/> and <paramref name="value"/>. If <see langword="null"/> is passed, the default comparer will be used.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see langword="true"/> if the value was different from the <paramref name="storage"/> variable and events on <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> were raised; otherwise, <see langword="false"/>.</returns>
        protected virtual bool SetProperty<T>(ref WeakReference<T>? storage, T? value, out T? oldValue, IEqualityComparer<T?>? comparer = null, [CallerMemberName] string? propertyName = null)
            where T: class
        {
            oldValue = storage?.TargetOrDefault();
            if ((comparer ?? EqualityComparer<T?>.Default).Equals(oldValue, value))
                return false;

            NotifyPropertyChanging(propertyName);
            storage = value is T tValue ? new WeakReference<T>(tValue) : default;
            NotifyPropertyChanged(propertyName);
            return true;
        }

        /// <summary>
        /// <para>
        /// Set <paramref name="storage"/> to a <see cref="WeakReference{T}"/> of the given <paramref name="value"/>.
        /// </para>
        /// <para>
        /// If the given <paramref name="value"/> is different than the current value,
        /// it raises an event on <see cref="INotifyPropertyChanging.PropertyChanging"/> before the storage changes and <see cref="INotifyPropertyChanged.PropertyChanged"/> after the storage was changed.
        /// </para>
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="storage">Reference to the storage field containing the <see cref="WeakReference{T}"/>.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="comparer">An optional comparer to compare the value of <paramref name="storage"/> and <paramref name="value"/>. If <see langword="null"/> is passed, the default comparer will be used.</param>
        /// <param name="propertyName">Name of the property.</param>
        /// <returns><see langword="true"/> if the value was different from the <paramref name="storage"/> variable and events on <see cref="PropertyChanging"/> and <see cref="PropertyChanged"/> were raised; otherwise, <see langword="false"/>.</returns>
        protected bool SetProperty<T>(ref WeakReference<T>? storage, T? value, IEqualityComparer<T?>? comparer = null, [CallerMemberName] string? propertyName = null)
            where T : class
        {
            return SetProperty(ref storage, value, out _, comparer, propertyName);
        }
    }
}