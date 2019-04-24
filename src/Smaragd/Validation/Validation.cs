using System;

namespace NKristek.Smaragd.Validation
{
    /// <inheritdoc />
    /// <remarks>
    /// Validates a given value with a generic type <typeparamref name="T"/>.
    /// </remarks>
    /// <typeparam name="T">Type of the value to validate.</typeparam>
    public abstract class Validation<T>
        : IValidation<T>
    {
        /// <inheritdoc />
        public abstract bool IsValid(T value, out string errorMessage);

        /// <inheritdoc />
        /// <exception cref="ArgumentException">If the given object has not the correct type expected by this <see cref="Validation{T}"/>.</exception>
        bool IValidation.IsValid(object value, out string errorMessage)
        {
            if (typeof(T).IsValueType)
            {
                if (value == null)
                    throw new ArgumentException($"Value is null but type {typeof(T).Name} is a value type.");

                if (!(value is T typedValue))
                    throw new ArgumentException($"Value is not of type {typeof(T).Name}");

                return IsValid(typedValue, out errorMessage);
            }

            if (value == null)
                return IsValid(default(T), out errorMessage);

            if (!(value is T tValue))
                throw new ArgumentException($"Value is not of type {typeof(T).Name}");

            return IsValid(tValue, out errorMessage);
        }
    }
}