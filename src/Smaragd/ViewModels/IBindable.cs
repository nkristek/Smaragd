using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels
{
    /// <inheritdoc cref="INotifyPropertyChanged" />
    /// <summary>
    /// Notifies clients that a property value is changing or has changed.
    /// </summary>
    public interface IBindable 
        : INotifyPropertyChanging, INotifyPropertyChanged
    {
    }
}