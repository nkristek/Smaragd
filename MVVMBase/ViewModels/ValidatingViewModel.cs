using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace nkristek.MVVMBase.ViewModels
{
    public abstract class ValidatingViewModel
        : ViewModel, IDataErrorInfo
    {
        public ValidatingViewModel()
        {
            foreach (var property in GetType().GetTypeInfo().DeclaredProperties)
            {
                var validatingAttribute = property.GetCustomAttribute<InitiallyNotValidAttribute>();
                if (validatingAttribute == null)
                    continue;
                
                SetValidationError(validatingAttribute.Message, property.Name);
            }
        }

        /// <summary>
        /// If data in this <see cref="ViewModel"/> is valid
        /// </summary>
        public bool IsValid => _validationErrors.Count == 0;

        private readonly Dictionary<string, string> _validationErrors = new Dictionary<string, string>();

        /// <summary>
        /// Set the validation error of the property
        /// </summary>
        /// <param name="error">This is the validation error and has to be set to null if no validation error occured</param>
        /// <param name="propertyName">Name of the property which validates with this error</param>
        protected void SetValidationError(string error, [CallerMemberName] string propertyName = null)
        {
            if (propertyName == null)
                return;
            
            if (String.IsNullOrEmpty(error))
                _validationErrors.Remove(propertyName);
            else
                _validationErrors[propertyName] = error;
            
            RaisePropertyChanged(nameof(IsValid));
        }

        /// <summary>
        /// Concatenated validation errors
        /// </summary>
        public string Error
        {
            get { return String.Join(", ", _validationErrors.Select(kvp => $"{kvp.Key}: {kvp.Value}")); }
        }

        /// <summary>
        /// Get validation error by the name of the property
        /// </summary>
        /// <param name="columnName">Name of the property</param>
        /// <returns></returns>
        public string this[string columnName] => _validationErrors.ContainsKey(columnName) ? _validationErrors[columnName] : String.Empty;
    }
}
