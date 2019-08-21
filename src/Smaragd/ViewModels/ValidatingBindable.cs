using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IValidatingBindable" />
    public abstract class ValidatingBindable
        : Bindable, IValidatingBindable
    {
        private readonly Dictionary<string, IReadOnlyCollection<object>> _errors = new Dictionary<string, IReadOnlyCollection<object>>();

        /// <inheritdoc />
        public virtual bool HasErrors => _errors.Count > 0;

        /// <inheritdoc />
        public virtual IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(kvp => kvp.Value);
            return _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<object>();
        }

        /// <summary>
        /// Set validation errors of a property.
        /// </summary>
        /// <param name="errors">The errors of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/> or empty.</exception>
        protected virtual void SetErrors(IEnumerable errors, [CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            if (errors != null && errors.Cast<object>().Any())
            {
                NotifyPropertyChanging(nameof(HasErrors));
                _errors[propertyName] = errors.Cast<object>().ToList().AsReadOnly();
                NotifyErrorsChanged(propertyName);
                NotifyPropertyChanged(nameof(HasErrors));
            }
            else if (_errors.ContainsKey(propertyName))
            {
                NotifyPropertyChanging(nameof(HasErrors));
                _errors.Remove(propertyName);
                NotifyErrorsChanged(propertyName);
                NotifyPropertyChanged(nameof(HasErrors));
            }
        }

        /// <inheritdoc />
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raises an event on <see cref="INotifyDataErrorInfo.ErrorsChanged"/> to indicate that the validation errors have changed.
        /// </summary>
        /// <param name="propertyName">Property name of which the validation errors changed.</param>
        protected virtual void NotifyErrorsChanged([CallerMemberName] string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }
    }
}
