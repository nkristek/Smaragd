using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using nkristek.MVVMBase.Attributes;
using nkristek.MVVMBase.Commands;

namespace nkristek.MVVMBase.ViewModels
{
    /// <summary>
    /// Adds the functionality to use the <see cref="PropertySourceAttribute"/> above properties and <see cref="CommandCanExecuteSourceAttribute"/> above <see cref="IRaiseCanExecuteChanged"/> implementations
    /// </summary>
    public abstract class ComputedBindableBase
        : BindableBase
    {
        internal Dictionary<string, IList<Attribute>> CachedAttributes { get; } = new Dictionary<string, IList<Attribute>>();

        private readonly Dictionary<string, IList<string>> _propertyNamesToNotify = new Dictionary<string, IList<string>>();

        private readonly Dictionary<string, IList<string>> _commandNamesToNotify = new Dictionary<string, IList<string>>();

        public ComputedBindableBase()
        {
            var properties = GetType().GetProperties().ToList();
            foreach (var property in properties)
                CachedAttributes[property.Name] = property.GetCustomAttributes().ToList();

            var propertyNames = properties.Select(p => p.Name).ToList();
            InitPropertyNamesToNotify(propertyNames);
            InitCommandNamesToNotify(propertyNames);

            PropertyChanged += NotifyPropertiesOnPropertyChanged;
            PropertyChanged += NotifyCommandsOnPropertyChanged;
        }

        private void InitPropertyNamesToNotify(IList<string> propertyNames)
        {
            foreach (var propertyAttributes in CachedAttributes)
            {
                foreach (var attribute in propertyAttributes.Value.OfType<PropertySourceAttribute>())
                {
                    foreach (var sourceName in attribute.Sources)
                    {
                        // skip when there is no property with this name
                        if (!propertyNames.Contains(sourceName) || sourceName == propertyAttributes.Key)
                            continue;

                        if (!_propertyNamesToNotify.ContainsKey(sourceName))
                            _propertyNamesToNotify[sourceName] = new List<string>();

                        if (!_propertyNamesToNotify[sourceName].Contains(propertyAttributes.Key))
                            _propertyNamesToNotify[sourceName].Add(propertyAttributes.Key);
                    }
                }
            }
        }

        private void InitCommandNamesToNotify(IList<string> propertyNames)
        {
            foreach (var propertyAttributes in CachedAttributes)
            {
                foreach (var attribute in propertyAttributes.Value.OfType<CommandCanExecuteSourceAttribute>())
                {
                    foreach (var sourceName in attribute.Sources)
                    {
                        // skip when there is no property with this name
                        if (!propertyNames.Contains(sourceName))
                            continue;

                        if (!_commandNamesToNotify.ContainsKey(sourceName))
                            _commandNamesToNotify[sourceName] = new List<string>();

                        if (!_commandNamesToNotify[sourceName].Contains(propertyAttributes.Key))
                            _commandNamesToNotify[sourceName].Add(propertyAttributes.Key);
                    }
                }
            }
        }
        
        private void NotifyPropertiesOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_propertyNamesToNotify.ContainsKey(e.PropertyName))
                return;

            foreach (var propertyNameToNotify in _propertyNamesToNotify[e.PropertyName])
                RaisePropertyChanged(propertyNameToNotify);
        }

        private void NotifyCommandsOnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (!_commandNamesToNotify.ContainsKey(e.PropertyName))
                return;

            var type = GetType();
            foreach (var commandNameToNotify in _commandNamesToNotify[e.PropertyName])
            {
                try
                {
                    var value = type.GetProperty(commandNameToNotify)?.GetValue(this);
                    (value as IRaiseCanExecuteChanged)?.RaiseCanExecuteChanged();
                }
                catch { }
            }
        }
    }
}
