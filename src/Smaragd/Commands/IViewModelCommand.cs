using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="ICommand" />
    /// <summary>
    /// Defines a command with a context <typeparamref name="TViewModel"/>.
    /// </summary>
    /// <typeparam name="TViewModel">Context <see cref="IViewModel"/> of this command.</typeparam>
    public interface IViewModelCommand<TViewModel>
        : INamedCommand, IRaiseCanExecuteChanged, IBindableCommand where TViewModel : class, IViewModel
    {
        /// <summary>
        /// Parent of this <see cref="IViewModelCommand{TViewModel}"/>.
        /// </summary>
        TViewModel Parent { get; set; }
    }
}