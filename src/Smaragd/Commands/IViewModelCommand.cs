using System.Windows.Input;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc cref="ICommand" />
    /// <summary>
    /// Defines a command with a context <typeparamref name="TViewModel"/>.
    /// </summary>
    /// <typeparam name="TViewModel">The type of the context <see cref="IViewModel"/>.</typeparam>
    public interface IViewModelCommand<TViewModel>
        : IBindableCommand where TViewModel : class, IViewModel
    {
        /// <summary>
        /// Context of this <see cref="IViewModelCommand{TViewModel}"/>.
        /// </summary>
        TViewModel Context { get; set; }
    }
}