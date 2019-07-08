using System.ComponentModel;

namespace NKristek.Smaragd.ViewModels
{
    /// <summary>
    /// Notifies clients that a property value is changing or has changed.
    /// </summary>
    public interface IBindable 
        : INotifyPropertyChanging, INotifyPropertyChanged
    {
    }
}