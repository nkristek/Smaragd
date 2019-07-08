using System.Windows.Input;

namespace NKristek.Smaragd.Commands
{
    /// <inheritdoc />
    /// <remarks>
    /// Defines a <see cref="Name"/> property which uniquely identifies this <see cref="ICommand"/>.
    /// </remarks>
    public interface INamedCommand
        : ICommand
    {
        /// <summary>
        /// A name which uniquely identifies this <see cref="ICommand"/>.
        /// </summary>
        string Name { get; }
    }
}