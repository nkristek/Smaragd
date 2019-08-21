using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IValidatingViewModel" />
    public abstract class ValidatingViewModel
        : ViewModel, IValidatingViewModel
    {
        private readonly Dictionary<string, IReadOnlyCollection<object>> _errors = new Dictionary<string, IReadOnlyCollection<object>>();

        #region IValidatingViewModel

        /// <inheritdoc />
        [IsDirtyIgnored]
        [PropertySource(nameof(HasErrors))]
        public virtual bool IsValid => !HasErrors;

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/> or empty.</exception>
        public virtual void SetErrors(IEnumerable errors, [CallerMemberName] string propertyName = null)
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

        #endregion

        #region INotifyDataErrorInfo

        /// <inheritdoc />
        [IsDirtyIgnored]
        public virtual bool HasErrors => _errors.Any();

        /// <inheritdoc />
        public virtual IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return _errors.SelectMany(kvp => kvp.Value.Cast<object>());
            return _errors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<object>();
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

        #endregion
    }
}