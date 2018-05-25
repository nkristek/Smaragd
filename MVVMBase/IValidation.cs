namespace nkristek.MVVMBase
{
    public interface IValidation
    {
        bool IsValid(object value, out string errorMessage);
    }
}
