using System;
using System.Collections.Generic;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// Use this on properties in classes that are subclasses of ComputedBindableBase to indicate, on which properties this property depends.
    /// It will then raise a PropertyChanged event for this property too
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PropertySourceAttribute
        : Attribute
    {
        public IEnumerable<string> Sources { get; private set; }

        public PropertySourceAttribute(params string[] sources)
        {
            Sources = sources;
        }
    }
}
