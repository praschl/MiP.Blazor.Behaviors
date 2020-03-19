using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace MiP.Blazor.Behaviors
{
    internal static class ReflectionHelper
    {
        public static IReadOnlyCollection<(string Name, IBehavior Instance)> FindBehaviors(object instance)
        {
            return GetValuesImplementing<IBehavior>(instance);
        }

        public static IReadOnlyCollection<(string Name, INotifyPropertyChanged Instance)> FindNotifyPropertyChanged(object instance)
        {
            return GetValuesImplementing<INotifyPropertyChanged>(instance);
        }

        // injected items are always private properties
        // but we support properties and fields of any visibility, not all may be injected, because the could have parameters.

        // also it seems that properties from the razor component are returned in reverse order.
        // if the order of behaviors is important, they should be implemented in code-behind.

        private static IReadOnlyCollection<(string Name, TInterface Instance)> GetValuesImplementing<TInterface>(object instance)
        {
            Type type = instance.GetType();

            const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public;

            IEnumerable<(string name, TInterface instance, bool isProperty)> propertyValues = type
                .GetProperties(flags)
                .Where(p => typeof(TInterface).IsAssignableFrom(p.PropertyType))
                .Select(p => (p.Name, (TInterface)p.GetValue(instance), true));

            // hashset will get us rid of values from backing fields of auto properties.
            HashSet<TInterface> instances = new HashSet<TInterface>(propertyValues.Select(p => p.instance), new ReferenceComparer<TInterface>());

            IEnumerable<(string name, TInterface instance, bool isProperty)> fieldValues = type
                .GetFields(flags)
                .Where(f => typeof(TInterface).IsAssignableFrom(f.FieldType))
                .Select(f => (f.Name, instance: (TInterface)f.GetValue(instance), false))
                .Where(x => !instances.Contains(x.instance));

            return propertyValues.Concat(fieldValues)
                .Where(b => b.instance != null)
                .Select(b => (Name: b.name, Instance: b.instance))
                .ToArray();
        }
    }
}
