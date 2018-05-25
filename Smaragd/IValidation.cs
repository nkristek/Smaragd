namespace NKristek.Smaragd
{
    public interface IValidation
    {
        bool IsValid(object value, out string errorMessage);
    }
}
