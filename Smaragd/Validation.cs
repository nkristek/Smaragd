using System;

namespace NKristek.Smaragd
{
    public abstract class Validation<T>
        : IValidation
    {
        public abstract bool IsValid(T value, out string errorMessage);

        public bool IsValid(object value, out string errorMessage)
        {
            if (value != null && !(value is T))
                throw new ArgumentException($"Value is not of type {typeof(T).Name}");

            return IsValid((T)value, out errorMessage);
        }
    }
}
