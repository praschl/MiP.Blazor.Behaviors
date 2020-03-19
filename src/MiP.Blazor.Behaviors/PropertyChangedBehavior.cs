using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace MiP.Blazor.Behaviors
{
    /// <summary>
    /// This behavior updates the UI when one of the (injected) models raises a PropertyChanged event.
    /// </summary>
    /// <remarks>
    /// A component that has this behavior will be scanned for properties and fields that implement <see cref="INotifyPropertyChanged"/>.
    /// The behavior subscribes to all events of these properties and unsubscribes when the component is removed.
    /// Only root level properties will be used, nested properties are ignored.
    /// </remarks>
    public class PropertyChangedBehavior : Behavior<BehaviorComponent>
    {
        private IReadOnlyCollection<INotifyPropertyChanged> _propertyChangedItems;

        /// <summary>
        /// Subscribes to <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            _propertyChangedItems = ReflectionHelper.FindNotifyPropertyChanged(Component).Select(x => x.Instance).ToArray();

            foreach (var item in _propertyChangedItems)
            {
                item.PropertyChanged += Item_PropertyChanged;
            }
        }

        /// <summary>
        /// Unsubscribes from <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        protected override void OnComponentDisposed()
        {
            base.OnComponentDisposed();

            foreach (var item in _propertyChangedItems)
            {
                item.PropertyChanged -= Item_PropertyChanged;
            }
        }

        [SuppressMessage("Design", "CA1031:Do not catch general exception types", Justification = "async void event handler")]
        private void Item_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Component.OnPropertyChangedHandler(sender, e);
        }
    }
}
