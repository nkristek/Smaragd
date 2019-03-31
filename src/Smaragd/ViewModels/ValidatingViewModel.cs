using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using NKristek.Smaragd.Attributes;
using NKristek.Smaragd.Validation;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IValidatingViewModel" />
    public abstract class ValidatingViewModel
        : ViewModel, IValidatingViewModel
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

        #endregion

        #region INotifyDataErrorInfo

        /// <inheritdoc />
        [IsDirtyIgnored]
        public bool HasErrors => _validationErrors.Any();

        /// <inheritdoc />
        public IEnumerable GetErrors(string propertyName)
        {
            if (String.IsNullOrEmpty(propertyName))
                return GetAllErrors();
            return _validationErrors.TryGetValue(propertyName, out var errors) ? errors : Enumerable.Empty<string>();
        }

        /// <inheritdoc />
        public virtual event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        #endregion

        #region IRaiseErrorsChanged

        /// <inheritdoc />
        public virtual void RaiseErrorsChanged(string propertyName = null)
        {
            var argument = new DataErrorsChangedEventArgs(propertyName);
            ErrorsChanged?.Invoke(this, argument);
            RaisePropertyChanged(nameof(HasErrors));
        }

        #endregion

        #region IValidatingViewModel

        /// <inheritdoc />
        [IsDirtyIgnored]
        [PropertySource(nameof(HasErrors))]
        public virtual bool IsValid => !HasErrors;

        private bool _validationSuspended;

        /// <inheritdoc />
        public bool IsValidationSuspended
        {
            get => _validationSuspended;
            set
            {
                if (SetProperty(ref _validationSuspended, value, out _) && !_validationSuspended)
                    ValidateAll();
            }
        }

        /// <inheritdoc />
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

            if (!IsValidationSuspended)
                Validate(propertyName, initialValue, existingValidations.OfType<Validation<T>>());
        }

        /// <inheritdoc />
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

        /// <inheritdoc />
        public bool RemoveValidations<T>(Expression<Func<T>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            return _validations.Remove(propertyName);
        }

        /// <inheritdoc />
        public IEnumerable<KeyValuePair<string, IList<IValidation>>> Validations()
        {
            return _validations.ToList();
        }

        /// <inheritdoc />
        public IEnumerable<Validation<T>> Validations<T>(Expression<Func<T>> propertySelector)
        {
            var propertyName = GetPropertyName(propertySelector);
            return _validations.TryGetValue(propertyName, out var validationsOfProperty)
                ? validationsOfProperty.OfType<Validation<T>>()
                : Enumerable.Empty<Validation<T>>();
        }

        #endregion

        private IEnumerable<string> GetAllErrors()
        {
            return _validationErrors.SelectMany(kvp => kvp.Value).Where(e => !String.IsNullOrEmpty(e));
        }
        
        /// <inheritdoc />
        protected override void RaisePropertyChanged(string propertyName, IEnumerable<string> additionalPropertyNames)
        {
            var additionalPropertyNamesList = additionalPropertyNames.ToList();
            base.RaisePropertyChanged(propertyName, additionalPropertyNamesList);

            if (IsValidationSuspended)
                return;

            var type = GetType();
            foreach (var propertyNameToValidate in Enumerable.Repeat(propertyName, 1).Concat(additionalPropertyNamesList))
            {
                if (!_validations.TryGetValue(propertyNameToValidate, out var validations))
                    continue;

                var valueProperty = type.GetProperty(propertyNameToValidate);
                if (valueProperty == null)
                    continue;

                var value = valueProperty.GetValue(this, null);
                Validate(propertyNameToValidate, value, validations);
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

        private static string GetPropertyName<T>(Expression<Func<T>> propertyExpression)
        {
            if (!(propertyExpression.Body is MemberExpression memberExpression))
                throw new ArgumentException("Expression body is not of type MemberExpression");
            return memberExpression.Member.Name;
        }
    }
}