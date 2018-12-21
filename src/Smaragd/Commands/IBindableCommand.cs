using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="ICommand" />
    /// <summary>
    /// Defines a command with <see cref="T:System.ComponentModel.INotifyPropertyChanging" /> and <see cref="T:System.ComponentModel.INotifyPropertyChanged" /> support.
    /// </summary>
    public interface IBindableCommand
        : ICommand, IRaisePropertyChanging, IRaisePropertyChanged
    {
    }
}