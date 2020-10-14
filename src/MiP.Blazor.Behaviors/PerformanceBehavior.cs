using System.Diagnostics;

namespace MiP.Blazor.Behaviors
{
    /// <summary>
    /// This behavior measures the time it takes to render the component.
    /// </summary>
    public class PerformanceBehavior : Behavior
    {
        private Stopwatch _stopwatch;

        /// <summary>
        /// Starts monitoring the time it takes to render the component, except on first render,
        /// and only if the component <paramref name="willRender"/>.
        /// </summary>
        /// <param name="willRender"></param>
        protected override void OnBeforeRender(bool willRender)
        {
            if (!willRender)
                return;

            // NOTE: When the component is rendered the first time, this method is not called,
            // because ShouldRender() is never called by Blazor.

            _stopwatch = Stopwatch.StartNew();
        }

        /// <summary>
        /// Writes the time it took to render the component to the Debug log.
        /// </summary>
        protected override void OnAfterRender(bool firstRender)
        {
            if (_stopwatch != null)
            {
                Debug.WriteLine($"RENDER TIME: {Component}: {_stopwatch.ElapsedMilliseconds}ms");
            }
        }
    }
}
