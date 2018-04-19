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
            var declaredProperties = GetType().GetTypeInfo().DeclaredProperties.ToList();

            // PropertySourceAttribute
            var propertiesWithPropertiesToNotify = new Dictionary<string, List<string>>();
            foreach (var property in declaredProperties)
            {
                // get the PropertySource attribute from the property, if it exists this property should be notified from the source properties listed in the attribute
                var computedAttribute = property.GetCustomAttribute<PropertySourceAttribute>();
                if (computedAttribute == null)
                    continue;

                foreach (var sourceName in computedAttribute.Sources)
                {
                    // skip when there is no property with this name
                    if (declaredProperties.All(p => p.Name != sourceName))
                        continue;

                    // create a new entry in the dictionary if this property doesn't notify another property already
                    if (!propertiesWithPropertiesToNotify.ContainsKey(sourceName))
                        propertiesWithPropertiesToNotify[sourceName] = new List<string>();

                    // add the property to the list of properties which get notified
                    propertiesWithPropertiesToNotify[sourceName].Add(property.Name);
                }
            }
            
            // CommandCanExecuteSourceAttribute
            var propertiesWithCommandsToNotify = new Dictionary<string, List<string>>();
            foreach (var property in declaredProperties)
            {
                // get the CommandCanExecuteSource attribute from the property, if it exists this command should be notified from the source properties listed in the attribute
                var computedAttribute = property.GetCustomAttribute<CommandCanExecuteSourceAttribute>();
                if (computedAttribute == null)
                    continue;

                foreach (var sourceName in computedAttribute.Sources)
                {
                    // skip when there is no property with this name
                    if (declaredProperties.All(p => p.Name != sourceName))
                        continue;

                    // create a new entry in the dictionary if this property doesn't notify another command already
                    if (!propertiesWithCommandsToNotify.ContainsKey(sourceName))
                        propertiesWithCommandsToNotify[sourceName] = new List<string>();

                    // add the command to the list of commands which get notified
                    propertiesWithCommandsToNotify[sourceName].Add(property.Name);
                }
            }

            PropertyChanged += (sender, e) => {
                if (propertiesWithPropertiesToNotify.ContainsKey(e.PropertyName))
                {
                    foreach (var propertyNameToNotify in propertiesWithPropertiesToNotify[e.PropertyName])
                        RaisePropertyChanged(propertyNameToNotify);
                }

                if (!propertiesWithCommandsToNotify.ContainsKey(e.PropertyName))
                    return;

                var type = GetType();
                foreach (var commandNameToNotify in propertiesWithCommandsToNotify[e.PropertyName])
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
