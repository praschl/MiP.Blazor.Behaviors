using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading.Tasks;

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
        private static readonly IEqualityComparer<INotifyPropertyChanged> _comparer = new ReferenceComparer<INotifyPropertyChanged>();

        private readonly HashSet<INotifyPropertyChanged> _propertyChangedItems = new HashSet<INotifyPropertyChanged>(_comparer);

        private readonly object _lock = new object();

        /// <summary>
        /// Subscribes to <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            UpdateEvents();
        }

        /// <summary>
        /// Subscribes to <see cref="INotifyPropertyChanged"/>.
        /// </summary>
        protected async override Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync().ConfigureAwait(false);

            UpdateEvents();
        }

        private void UpdateEvents()
        {
            var currentInstances = ReflectionHelper.FindNotifyPropertyChanged(Component)
                .Select(x => x.Instance);

            var oldItems = _propertyChangedItems.Except(currentInstances, _comparer).ToArray();
            foreach (var oldItem in oldItems)
            {
                oldItem.PropertyChanged -= Item_PropertyChanged;
                _propertyChangedItems.Remove(oldItem);
            }

            var newItems = currentInstances.Except(_propertyChangedItems, _comparer).ToArray();
            foreach (var newItem in newItems)
            {
                newItem.PropertyChanged += Item_PropertyChanged;
                _propertyChangedItems.Add(newItem);
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
