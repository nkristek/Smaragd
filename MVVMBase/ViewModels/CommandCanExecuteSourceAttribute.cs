using System;
using System.Collections.Generic;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// Use this on BindableCommands or AsyncBindableCommands in classes that are subclasses of ComputedBindableBase to indicate, on which properties the CanExecute method of the command depends.
    /// It will then raise a CanExecuteChanged event on the command, once a property changes
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CommandCanExecuteSourceAttribute
        : Attribute
    {
        public IEnumerable<string> Sources { get; private set; }

        public CommandCanExecuteSourceAttribute(params string[] sources)
        {
            Sources = sources;
        }
    }
}
