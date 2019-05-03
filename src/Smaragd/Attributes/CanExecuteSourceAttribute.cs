using System;
using System.Collections.Generic;
using System.Windows.Input;
using NKristek.Smaragd.Commands;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// This <see cref="Attribute"/> can be used on any method called "CanExecute" in a class inheriting from either <see cref="ViewModelCommand{TViewModel}"/> or <see cref="AsyncViewModelCommand{T}"/>.
    /// It indicates, that the result of <see cref="ICommand.CanExecute(object)" /> depends on the named properties.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public sealed class CanExecuteSourceAttribute
        : Attribute
    {
        /// <summary>
        /// Property names of the parent which should raise <see cref="ICommand.CanExecuteChanged"/>.
        /// </summary>
        public IEnumerable<string> PropertySources { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="CanExecuteSourceAttribute" /> class with one or multiple names of properties <see cref="ICommand.CanExecute"/> depends on.
        /// </summary>
        /// <param name="propertyNames">Names of source properties.</param>
        /// <exception cref="ArgumentNullException"><paramref name="propertyNames"/> is <see langword="null"/>.</exception>
        public CanExecuteSourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
        }
    }
}