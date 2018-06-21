using System;

namespace NKristek.Smaragd.Validation
{
    /// <inheritdoc />
    /// <summary>
    /// <see cref="IValidation"/> with a generic type
    /// </summary>
    /// <typeparam name="T">Type of the value to validate</typeparam>
    public abstract class Validation<T>
        : IValidation
    {
        /// <summary>
        /// Validates the given <paramref name="value"/>
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="errorMessage">Optional error message</param>
        /// <returns>If the value is valid</returns>
        public abstract bool IsValid(T value, out string errorMessage);

        /// <inheritdoc />
        public bool IsValid(object value, out string errorMessage)
        {
            if (value != null && !(value is T))
                throw new ArgumentException($"Value is not of type {typeof(T).Name}");

            return IsValid((T)value, out errorMessage);
        }
    }
}
