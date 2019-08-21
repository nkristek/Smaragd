using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="IBindable" />
    /// <summary>
    /// Notifies clients that errors of a property have changed.
    /// </summary>
    public interface IValidatingBindable
        : IBindable, INotifyDataErrorInfo
    {
    }
}
