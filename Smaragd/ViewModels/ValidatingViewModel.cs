using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="ViewModel" />
    /// <remarks>
    /// This class adds an implementation of <see cref="T:System.ComponentModel.IDataErrorInfo" /> and <see cref="T:System.ComponentModel.INotifyDataErrorInfo" />.
    /// </remarks>
    public abstract class ValidatingViewModel
        : ViewModel, IDataErrorInfo, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, IList<IValidation>> _validations = new Dictionary<string, IList<IValidation>>();

        private readonly Dictionary<string, IList<string>> _validationErrors = new Dictionary<string, IList<string>>();

        #region IDataErrorInfo

        /// <inheritdoc />
        public string this[string propertyName]
        {
            get
            {
                if (String.IsNullOrEmpty(propertyName))
                    return Error;
                return _validationErrors.TryGetValue(propertyName, out var errors) ? String.Join(Environment.NewLine, errors) : null;
            }
        }

        /// <inheritdoc />
        public string Error
        {
            get
            {
                var errors = GetAllErrors().ToList();
                return errors.Any() ? String.Join(Environment.NewLine, errors) : null;
            }
        }

        private IEnumerable<string> GetAllErrors()
        {
            return _validationErrors.SelectMany(kvp => kvp.Value).Where(e => !String.IsNullOrEmpty(e));
        }

        #endregion

        #region INotifyDataErrorInfo

        /// <inheritdoc />
        [IsDirtyIgnored]
        public bool HasErrors => _validationErrors.Any();

        /// <inheritdoc />
        public System.Collections.IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return GetAllErrors();
            return _validationErrors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public virtual event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        /// <summary>
        /// Raises an event on the <see cref="ErrorsChanged"/> event.
        /// </summary>
        /// <param name="propertyName">Property name of which the validation errors changed.</param>
        protected virtual void RaiseErrorsChanged(string propertyName = null)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            RaisePropertyChanged(nameof(HasErrors));
        }

        #endregion

        /// <summary>
        /// If data in this <see cref="ViewModel"/> is valid.
        /// </summary>
        [IsDirtyIgnored]
        [PropertySource(nameof(HasErrors))]
        public bool IsValid => !HasErrors;

        /// <summary>
        /// Add a validation for the property returned by the lambda expression
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="propertySelector">Lambda expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">Validation to add</param>
        public void AddValidation<T>(Expression<Func<T>> propertySelector, Validation<T> validation)
        {
            var propertyName = GetPropertyName(propertySelector);
            var initialValue = propertySelector.Compile()();

            if (_validations.TryGetValue(propertyName, out var existingValidations))
            {
                existingValidations.Add(validation);
            }
            else
            {
                existingValidations = new List<IValidation> {validation};
                _validations.Add(propertyName, existingValidations);
            }

            if (!ValidationSuspended)
                Validate(propertyName, initialValue, existingValidations.OfType<Validation<T>>());
        }

        private void Validate(string propertyName, object value, IEnumerable<IValidation> validations)
        {
            var errors = new List<string>();
            foreach (var validation in validations)
            {
                if (!validation.IsValid(value, out var errorMessage))
                    errors.Add(errorMessage);
            }

            SetValidationErrors(propertyName, errors);
        }

        private void Validate<T>(string propertyName, T value, IEnumerable<Validation<T>> validations)
        {
            var errors = new List<string>();
            foreach (var validation in validations)
            {
                if (!validation.IsValid(value, out var errorMessage))
                    errors.Add(errorMessage);
            }

            SetValidationErrors(propertyName, errors);
        }

        private void SetValidationErrors(string propertyName, IEnumerable<string> errors)
        {
            var errorList = errors.Where(e => !String.IsNullOrEmpty(e)).ToList();
            if (errorList.Any())
            {
                _validationErrors[propertyName] = errorList;
                RaiseErrorsChanged(propertyName);
            }
            else if (_validationErrors.Remove(propertyName))
            {
                RaiseErrorsChanged(propertyName);
            }
        }

        /// <summary>
        /// Removes a specific validation for the property returned by the expression
        /// </summary>
        /// <typeparam name="T">Type of the property to validate</typeparam>
        /// <param name="propertySelector">Expression to select the property. eg.: () => MyProperty</param>
        /// <param name="validation">Validation to remove</param>
        /// <returns>If the validation was found and successfully removed</returns>
        public bool RemoveValidation<T>(Expression<Func<T>> propertySelector, Validation<T> validation)
        {
            var propertyName = GetPropertyName(propertySelector);
            if (!_validations.TryGetValue(propertyName, out var validationsOfProperty))
                return false;

            var validationWasRemoved = validationsOfProperty.Remove(validation);
            if (!validationsOfProperty.Any())
                _validations.Remove(propertyName);
            return validationWasRemoved;
        }

        /// <summary>
        /// Removes all validations for the property returned by the expression
        /// </summary>
        /// <typeparam name="T">Type of the validating property</typeparam>
        /// <param name="propertySelector">Expression to select the property. eg.: () => MyProperty</param>
        /// <returns>If the validation was found and successfully removed</returns>
        public bool RemoveValidations<T>(Expression<Func<T>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            return _validations.Remove(propertyName);
        }

        /// <summary>
        /// Get all validations. Key is the name of the property, value are all validations for the property.
        /// </summary>
        /// <returns>All validations. Key is the name of the property, value are all validations for the property.</returns>
        public IEnumerable<KeyValuePair<string, IList<IValidation>>> Validations()
        {
            return _validations.ToList();
        }

        /// <summary>
        /// Get all validations for the property returned by the expression
        /// </summary>
        /// <typeparam name="T">Type of the validating property</typeparam>
        /// <param name="propertySelector">Expression to select the property. eg.: () => MyProperty</param>
        /// <returns>All validations for the property</returns>
        public IEnumerable<Validation<T>> Validations<T>(Expression<Func<T>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            return _validations.TryGetValue(propertyName, out var validationsOfProperty)
                ? validationsOfProperty.OfType<Validation<T>>()
                : Enumerable.Empty<Validation<T>>();
        }

        private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression))
                throw new ArgumentException("Expression body is not of type MemberExpression");
            return memberExpression.Member.Name;
        }

        /// <inheritdoc />
        /// <remarks>
        /// Setting a property will also execute the appropriate validations for this property.
        /// </remarks>
        protected override bool SetProperty<T>(ref T storage, T value, out T oldValue, [CallerMemberName] string propertyName = "")
        {
            var propertyWasChanged = base.SetProperty(ref storage, value, out oldValue, propertyName);
            if (propertyWasChanged && !ValidationSuspended && _validations.TryGetValue(propertyName, out var validations))
                Validate(propertyName, value, validations.OfType<Validation<T>>());
            return propertyWasChanged;
        }

        private bool _validationSuspended;

        /// <summary>
        /// If validation is temporarily suspended.
        /// </summary>
        public bool ValidationSuspended
        {
            get => _validationSuspended;
            internal set
            {
                if (SetProperty(ref _validationSuspended, value, out _) && !_validationSuspended)
                    ValidateAll();
            }
        }

        private void ValidateAll()
        {
            var type = GetType();
            foreach (var propertyValidation in _validations)
            {
                var valueProperty = type.GetProperty(propertyValidation.Key);
                if (valueProperty == null)
                    continue;

                var value = valueProperty.GetValue(this, null);
                Validate(propertyValidation.Key, value, propertyValidation.Value);
            }
        }

        /// <summary>
        /// Temporarily suspends validation. This could be used in a batch update to prevent validation overhead.
        /// </summary>
        /// <returns><see cref="IDisposable"/> which reactivates automatic validation when disposed.</returns>
        public IDisposable SuspendValidation()
        {
            return new SuspendValidationDisposable(this);
        }
    }
}