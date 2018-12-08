using System;
using System.Collections.Generic;
using System.Windows.Input;
using NKristek.Smaragd.Commands;
using NKristek.Smaragd.ViewModels;

namespace NKristek.Smaragd.Attributes
{
    /// <inheritdoc />
    /// <summary>
    /// <para>
    /// This <see cref="Attribute"/> can be used on any method called "CanExecute" in a class inheriting from either <see cref="ViewModelCommand{TViewModel}"/> or <see cref="AsyncViewModelCommand{T}"/>.
    /// It indicates, that the result of <see cref="M:System.Windows.Input.ICommand.CanExecute(System.Object)" /> depends on the properties named.
    /// </para>
    /// <para>
    /// <see cref="E:System.Windows.Input.ICommand.CanExecuteChanged" /> will be automatically raised, when <see cref="E:System.ComponentModel.INotifyPropertyChanged.PropertyChanged" /> is raised for one of the properties.
    /// </para>
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class CanExecuteSourceAttribute
        : Attribute
    {
        /// <summary>
        /// Property names of the parent <see cref="ViewModel"/> which should raise <see cref="M:System.Windows.Input.ICommand.CanExecuteChanged"/>.
        /// </summary>
        public IEnumerable<string> PropertySources { get; }

        /// <inheritdoc />
        /// <summary>
        /// Initializes a new instance of the <see cref="T:NKristek.Smaragd.Attributes.CanExecuteSourceAttribute" /> class with one or multiple names of properties <see cref="ICommand.CanExecute"/> depends on.
        /// </summary>
        /// <param name="propertyNames">Names of source properties.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="propertyNames"/> is null.</exception>
        public CanExecuteSourceAttribute(params string[] propertyNames)
        {
            PropertySources = propertyNames ?? throw new ArgumentNullException(nameof(propertyNames));
        }
    }
}
