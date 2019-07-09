using System;
using System.Collections;
using System.Collections.Generic;
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
        #region IValidatingViewModel

        private readonly Dictionary<string, IList<string>> _validationErrors = new Dictionary<string, IList<string>>();

        /// <inheritdoc />
        [IsDirtyIgnored]
        [PropertySource(nameof(HasErrors))]
        public virtual bool IsValid => !HasErrors;

        /// <inheritdoc />
        /// <exception cref="ArgumentNullException"><paramref name="propertyName"/> is <see langword="null"/> or empty.</exception>
        public virtual void SetErrors(IEnumerable<string> errors, [CallerMemberName] string propertyName = null)
        {
            if (String.IsNullOrEmpty(propertyName))
                throw new ArgumentNullException(nameof(propertyName));

            var errorList = (errors ?? Enumerable.Empty<string>()).ToList();
            if (errorList.Any())
            {
                NotifyPropertyChanging(nameof(HasErrors));
                _validationErrors[propertyName] = errorList;
                NotifyErrorsChanged(propertyName);
                NotifyPropertyChanged(nameof(HasErrors));
            }
            else if (_validationErrors.ContainsKey(propertyName))
            {
                NotifyPropertyChanging(nameof(HasErrors));
                _validationErrors.Remove(propertyName);
                NotifyErrorsChanged(propertyName);
                NotifyPropertyChanged(nameof(HasErrors));
            }
        }

        #endregion

        #region INotifyDataErrorInfo

        /// <inheritdoc />
        [IsDirtyIgnored]
        public virtual bool HasErrors => _validationErrors.Any();

        /// <inheritdoc cref="INotifyDataErrorInfo.GetErrors" />
        public virtual IEnumerable<string> GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return _validationErrors.SelectMany(kvp => kvp.Value);
            return _validationErrors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        }

        IEnumerable INotifyDataErrorInfo.GetErrors(string propertyName)
        {
            return GetErrors(propertyName);
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