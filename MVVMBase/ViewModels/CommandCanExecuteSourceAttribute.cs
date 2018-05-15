using System;
using System.Collections.Generic;
using System.Windows.Input;
using nkristek.MVVMBase.Commands;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// Use this on <see cref="BindableCommand"/> or <see cref="AsyncBindableCommand"/> properties in classes that are subclasses of <see cref="ComputedBindableBase"/> to indicate, on which properties the <see cref="ICommand.CanExecute"/> method of the command depends.
    /// It will then raise a CanExecuteChanged event on the command, once a property changes.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandCanExecuteSourceAttribute
        : Attribute
    {
        public IEnumerable<string> Sources { get; }

        public CommandCanExecuteSourceAttribute(params string[] sources)
        {
            Sources = sources;
        }
    }
}
