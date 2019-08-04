using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="ICommand" />
    /// <summary>
    /// Defines a command that notifies clients that a property value is changing or has changed.
    /// </summary>
    public interface IBindableCommand
        : ICommand, IBindable
    {
    }
}