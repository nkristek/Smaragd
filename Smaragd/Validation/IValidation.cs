namespace NKristek.Smaragd.Validation
{
    /// <summary>
    /// Validation of values
    /// </summary>
    public interface IValidation
    {
        /// <summary>
        /// Validates the given <paramref name="value"/>
        /// </summary>
        /// <param name="value">Value to validate</param>
        /// <param name="errorMessage">Optional error message</param>
        /// <returns>If the value is valid</returns>
        bool IsValid(object value, out string errorMessage);
    }
}
