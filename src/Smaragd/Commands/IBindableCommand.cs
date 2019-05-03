using System.ComponentModel;
using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="ICommand" />
    /// <summary>
    /// Defines a command with <see cref="INotifyPropertyChanging" /> and <see cref="INotifyPropertyChanged" /> support.
    /// </summary>
    public interface IBindableCommand
        : ICommand, IRaisePropertyChanging, IRaisePropertyChanged
    {
    }
}