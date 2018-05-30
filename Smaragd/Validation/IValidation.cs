namespace NKristek.Smaragd.Validation
{
    public interface IValidation
    {
        bool IsValid(object value, out string errorMessage);
    }
}
