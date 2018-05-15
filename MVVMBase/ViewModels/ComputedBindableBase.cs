using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using nkristek.MVVMBase.Commands;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// Adds the functionality to use the <see cref="PropertySourceAttribute"/> above properties and <see cref="CommandCanExecuteSourceAttribute"/> above <see cref="IRaiseCanExecuteChanged"/> implementations
    /// </summary>
    public abstract class ComputedBindableBase
        : BindableBase
    {
        public ComputedBindableBase()
        {
            var properties = GetType().GetProperties().ToList();

            // PropertySourceAttribute
            var propertyNamesWithPropertyNamesToNotify = new Dictionary<string, List<string>>();
            foreach (var property in properties)
            {
                foreach (var propertySourceAttribute in property.GetCustomAttributes<PropertySourceAttribute>())
                {
                    foreach (var sourceName in propertySourceAttribute.Sources)
                    {
                        // skip when there is no property with this name
                        if (properties.All(p => p.Name != sourceName))
                            continue;
                        
                        if (!propertyNamesWithPropertyNamesToNotify.ContainsKey(sourceName))
                            propertyNamesWithPropertyNamesToNotify[sourceName] = new List<string>();
                        if (!propertyNamesWithPropertyNamesToNotify[sourceName].Contains(property.Name))
                            propertyNamesWithPropertyNamesToNotify[sourceName].Add(property.Name);
                    }
                }
            }
            
            // CommandCanExecuteSourceAttribute
            var propertyNamesWithCommandNamesToNotify = new Dictionary<string, List<string>>();
            foreach (var property in properties)
            {
                foreach (var commandCanExecuteSourceAttribute in property.GetCustomAttributes<CommandCanExecuteSourceAttribute>())
                {
                    foreach (var sourceName in commandCanExecuteSourceAttribute.Sources)
                    {
                        // skip when there is no property with this name
                        if (properties.All(p => p.Name != sourceName))
                            continue;
                        
                        if (!propertyNamesWithCommandNamesToNotify.ContainsKey(sourceName))
                            propertyNamesWithCommandNamesToNotify[sourceName] = new List<string>();
                        if (!propertyNamesWithCommandNamesToNotify[sourceName].Contains(property.Name))
                            propertyNamesWithCommandNamesToNotify[sourceName].Add(property.Name);
                    }
                }
            }

            PropertyChanged += (sender, e) => {
                if (propertyNamesWithPropertyNamesToNotify.ContainsKey(e.PropertyName))
                {
                    foreach (var propertyNameToNotify in propertyNamesWithPropertyNamesToNotify[e.PropertyName])
                        RaisePropertyChanged(propertyNameToNotify);
                }

                if (!propertyNamesWithCommandNamesToNotify.ContainsKey(e.PropertyName))
                    return;

                var type = GetType();
                foreach (var commandNameToNotify in propertyNamesWithCommandNamesToNotify[e.PropertyName])
                {
                    try
                    {
                        var value = type.GetProperty(commandNameToNotify)?.GetValue(this);
                        (value as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
                    }
                    catch { }
                }
            };
        }
    }
}
