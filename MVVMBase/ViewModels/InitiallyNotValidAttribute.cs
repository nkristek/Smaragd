using System;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// Use this on properties in classes that are subclasses of <see cref="ValidatingViewModel"/> to indicate that this property uses validation and is initially not valid.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class InitiallyNotValidAttribute
        : Attribute
    {
        public string Message { get; }

        public InitiallyNotValidAttribute(string message)
        {
            Message = message;
        }
    }
}
