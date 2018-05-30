using System;

namespace NKristek.Smaragd.Validation
{
    public class PredicateValidation<T>
        : Validation<T>
    {
        private readonly Predicate<T> _predicate;

        private readonly string _errorMessage;

        public PredicateValidation(Predicate<T> predicate, string errorMessage)
        {
            _predicate = predicate;
            _errorMessage = errorMessage;
        }

        public override bool IsValid(T value, out string errorMessage)
        {
            errorMessage = null;
            if (_predicate(value))
                return true;
            errorMessage = _errorMessage;
            return false;
        }
    }
}
