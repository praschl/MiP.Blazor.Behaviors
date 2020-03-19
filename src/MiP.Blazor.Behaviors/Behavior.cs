using Microsoft.AspNetCore.Components;
using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace MiP.Blazor.Behaviors
{
    /// <summary>
    /// Derive from this class to create your own behavior with a typed component.
    /// </summary>
    /// <typeparam name="T">
    /// Components using this behavior have to be of <typeparamref name="T"/> or from a derived type, or implement <typeparamref name="T"/>, if it is an interface.
    /// </typeparam>
    public class Behavior<T> : IBehavior
    {
        private T _component;

        /// <summary>
        /// Gets the component that uses this behavior.
        /// </summary>
        [SuppressMessage("Globalization", "CA1303:Do not pass literals as localized parameters")]
        protected T Component
        {
            get => _component;
            private set
            {
                if (_component != null && !ReferenceEquals(_component, value))
                    throw new InvalidOperationException("An behaviors instance must not be shared between components. If you injected the behavior, register it with transient lifetime.");

                _component = value;
            }
        }

        /// <summary>
        /// Get the name of the member that holds the instance of the behavior.
        /// </summary>
        public string MemberName { get; private set; }

        /// <summary>
        /// Called by <see cref="Component"/>s <see cref="ComponentBase.OnParametersSet"/>.
        /// </summary>
        protected virtual void OnParametersSet()
        {
        }

        /// <summary>
        /// Called by <see cref="Component"/>s <see cref="ComponentBase.OnInitialized"/>.
        /// </summary>
        protected virtual void OnInitialized()
        {
        }

        /// <summary>
        /// Called by <see cref="Component"/>s <see cref="ComponentBase.OnAfterRender"/>.
        /// </summary>
        protected virtual void OnAfterRender(bool firstRender)
        {
        }

        /// <summary>
        /// Called by <see cref="Component"/>s <see cref="BehaviorComponent.Dispose(bool)"/>.
        /// </summary>
        protected virtual void OnComponentDisposed()
        {
        }

        // async

        /// <summary>
        /// Called by <see cref="Component"/>s <see cref="ComponentBase.OnParametersSetAsync"/>.
        /// </summary>
        protected virtual Task OnParametersSetAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by <see cref="Component"/>s <see cref="ComponentBase.OnInitializedAsync"/>.
        /// </summary>
        protected virtual Task OnInitializedAsync()
        {
            return Task.CompletedTask;
        }

        /// <summary>
        /// Called by <see cref="Component"/>s <see cref="ComponentBase.OnAfterRenderAsync"/>.
        /// </summary>
        protected virtual Task OnAfterRenderAsync(bool firstRender)
        {
            return Task.CompletedTask;
        }

        // interface

        object IBehavior.Component
        {
            get => Component;
            set => Component = (T)value;
        }

        string IBehavior.MemberName
        {
            get => MemberName;
            set => MemberName = value;
        }

        void IBehavior.OnInitialized()
        {
            OnInitialized();
        }

        Task IBehavior.OnInitializedAsync()
        {
            return OnInitializedAsync();
        }

        void IBehavior.OnParametersSet()
        {
            OnParametersSet();
        }

        Task IBehavior.OnParametersSetAsync()
        {
            return OnParametersSetAsync();
        }

        void IBehavior.OnAfterRender(bool firstRender)
        {
            OnAfterRender(firstRender);
        }

        Task IBehavior.OnAfterRenderAsync(bool firstRender)
        {
            return OnAfterRenderAsync(firstRender);
        }

        void IBehavior.ComponentDisposed()
        {
            OnComponentDisposed();
        }
    }

    /// <summary>
    /// Derive from this class to create your own behavior that supports all components. 
    /// The component must derive from <see cref="BehaviorComponent"/>, or behaviors will not be found anyway.
    /// </summary>
    public class Behavior : Behavior<BehaviorComponent>
    {
        // simple behavior that can work with every component
    }
}
