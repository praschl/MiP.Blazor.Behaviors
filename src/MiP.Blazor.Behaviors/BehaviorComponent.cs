using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Timers;

namespace MiP.Blazor.Behaviors
{
    /// <summary>
    /// Base component for enabling behaviors. If you want to use behaviors, derive your component from this class.
    /// </summary>
    public class BehaviorComponent : ComponentBase, IDisposable
    {
        private IReadOnlyCollection<IBehavior> _behaviors;

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// </summary>
        /// <remarks>
        /// This method finds behaviors attached to the component,initializes them and calls <see cref="IBehavior.OnInitialized"/>.
        /// </remarks>
        protected override void OnInitialized()
        {
            var behaviorsWithName = ReflectionHelper.FindBehaviors(this);

            base.OnInitialized();

            List<IBehavior> behaviors = new List<IBehavior>();
            _behaviors = behaviors;

            foreach (var (name, instance) in behaviorsWithName)
            {
                behaviors.Add(instance);

                instance.Component = this;
                instance.MemberName = name;
                instance.OnInitialized();
            }
        }

        /// <summary>
        /// Method invoked when the component is ready to start, having received its
        /// initial parameters from its parent in the render tree.
        /// Override this method if you will perform an asynchronous operation and
        /// want the component to refresh when that operation is completed.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="IBehavior.OnInitializedAsync"/>.
        /// </remarks>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync().ConfigureAwait(false);

            foreach (var behavior in _behaviors)
            {
                await behavior.OnInitializedAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="IBehavior.OnParametersSet"/>.
        /// </remarks>
        protected override void OnParametersSet()
        {
            base.OnParametersSet();

            foreach (var behavior in _behaviors)
            {
                behavior.OnParametersSet();
            }
        }

        /// <summary>
        /// Method invoked when the component has received parameters from its parent in
        /// the render tree, and the incoming values have been assigned to properties.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="IBehavior.OnParametersSetAsync"/>.
        /// </remarks>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        protected override async Task OnParametersSetAsync()
        {
            await base.OnParametersSetAsync().ConfigureAwait(false);

            foreach (var behavior in _behaviors)
            {
                await behavior.OnParametersSetAsync().ConfigureAwait(false);
            }
        }

        /// <summary>
        /// Method invoked after each time the component has been rendered.
        /// </summary>
        /// <param name="firstRender">
        /// Set to <c>true</c> if this is the first time <see cref="ComponentBase.OnAfterRender(bool)" /> has been invoked
        /// on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <remarks>
        /// <para>
        /// The <see cref="ComponentBase.OnAfterRender(bool)" /> and <see cref="ComponentBase.OnAfterRenderAsync(bool)" /> lifecycle methods
        /// are useful for performing interop, or interacting with values recieved from <c>@ref</c>.
        /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        /// once.
        /// </para>
        /// <para>
        /// Calls <see cref="IBehavior.OnAfterRender(bool)"/>.
        /// </para>
        /// </remarks>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            foreach (var behavior in _behaviors)
            {
                behavior.OnAfterRender(firstRender);
            }
        }

        /// <summary>
        /// Method invoked after each time the component has been rendered. Note that the component does
        /// not automatically re-render after the completion of any returned <see cref="Task" />, because
        /// that would cause an infinite render loop.
        /// </summary>
        /// <param name="firstRender">
        /// Set to <c>true</c> if this is the first time <see cref="ComponentBase.OnAfterRender(bool)" /> has been invoked
        /// on this component instance; otherwise <c>false</c>.
        /// </param>
        /// <returns>A <see cref="Task" /> representing any asynchronous operation.</returns>
        /// <remarks>
        /// <para>
        /// The <see cref="ComponentBase.OnAfterRender(bool)" /> and <see cref="ComponentBase.OnAfterRenderAsync(bool)" /> lifecycle methods
        /// are useful for performing interop, or interacting with values recieved from <c>@ref</c>.
        /// Use the <paramref name="firstRender" /> parameter to ensure that initialization work is only performed
        /// once.
        /// </para>
        /// <para>
        /// Calls <see cref="IBehavior.OnAfterRenderAsync(bool)"/>.
        /// </para>
        /// </remarks>
        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender).ConfigureAwait(false);

            foreach (var behavior in _behaviors)
            {
                await behavior.OnAfterRenderAsync(firstRender).ConfigureAwait(false);
            }
        }

        // keeping this for later
        // result of all behaviors should be ORed together -> if one returns true -> render = true
        // if one returns true, the rest does not have to be called, here we are deciding only whether we have to Render
        // and shouldn't do any sideeffects.

        //protected override bool ShouldRender()
        //{
        //    return base.ShouldRender();
        //}

        /// <summary>
        /// Method invoked when the component is disposed.
        /// </summary>
        /// <remarks>
        /// Calls <see cref="IBehavior.ComponentDisposed"/>.
        /// </remarks>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            foreach (var behavior in _behaviors)
            {
                behavior.ComponentDisposed();
            }
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Handler method for the <see cref="PropertyChangedBehavior"/>.
        /// Handles property changed events by calling <see cref="ComponentBase.StateHasChanged"/> via <see cref="ComponentBase.InvokeAsync(Action)"/>
        /// Override to replace this with your own logic, for example to call <see cref="ComponentBase.StateHasChanged"/> without <see cref="ComponentBase.InvokeAsync(Action)"/>.
        /// </summary>
        /// <param name="sender">instance that raised the original event.</param>
        /// <param name="e">The original event args</param>
        protected internal virtual async void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
        {
            await InvokeAsync(StateHasChanged).ConfigureAwait(false);
        }

        /// <summary>
        /// Handler method for the <see cref="TimerBehavior"/>. Handles the <see cref="Timer.Elapsed"/> event. Default implementation does nothing.
        /// When overridden, the method can be marked as async void, but care must be taken, that no exception leaves the method.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected internal virtual void OnTimerElapsedHandler(object sender, ElapsedEventArgs e)
        {
        }
    }
}
