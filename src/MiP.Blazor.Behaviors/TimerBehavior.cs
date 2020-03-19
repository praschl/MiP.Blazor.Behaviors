using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Reflection;
using System.Timers;

namespace MiP.Blazor.Behaviors
{
    /// <summary>
    /// This behavior starts a timer with the <see cref="Interval"/> interval.
    /// </summary>
    /// <remarks>
    /// <para>
    /// You can handle a tick of the timer by
    /// </para>
    /// <para>
    /// a) overriding OnTimerElapsed on the <see cref="BehaviorComponent"/>.
    /// </para>
    /// <para>
    /// this is the preferred way, however this allows only one handler.
    /// If you have more than one <see cref="TimerBehavior"/>, you may want to handle each Elapsed event separately by
    /// </para>
    /// <para>
    /// b) implementing a method with the name being "{name of the member that stores the behavior}_TimerElapsedHandler", like this:
    /// </para>
    /// <code>
    /// private TimerBehavior _myTimer = new TimerBehavior { Interval = TimeSpan.FromSeconds(1) };
    ///
    /// private void _myTimer_TimerElapsedHandler(object sender, System.Timers.ElapsedEventArgs e)
    /// {
    ///   // ...
    /// }
    /// </code>
    /// <para>
    /// The member may be a field or property with any visibility.<br/>
    /// The method can be made async void, its just a handler for the <see cref="Timer.Elapsed"/> event, 
    /// however no exceptions should leave the method, as applies to all async void methods.
    /// </para>
    /// </remarks>
    [SuppressMessage("Design", "CA1001:Types that own disposable fields should be disposable", Justification = "Disposed in OnDisposed()")]
    public class TimerBehavior : Behavior<BehaviorComponent>
    {
        private const string TimerElapsedName = "_TimerElapsedHandler";

        private static readonly Type[] _handlerMethodParameters = new[] { typeof(object), typeof(ElapsedEventArgs) };

        private readonly Timer _timer = new Timer();
        private Action<object, ElapsedEventArgs> _customHandler;

        /// <summary>
        /// Gets or sets the interval at which the action is called.
        /// </summary>
        public TimeSpan Interval { get; set; }

        /// <summary>
        /// Initializes the component.
        /// </summary>
        protected override void OnInitialized()
        {
            base.OnInitialized();

            var componentType = Component.GetType();

            string customHandlerName = MemberName + TimerElapsedName;
            const BindingFlags flags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
            var customHandler = componentType.GetMethod(customHandlerName, flags, null, _handlerMethodParameters, null);

            if (customHandler != null)
            {
                var senderParam = Expression.Parameter(typeof(object));
                var eventArgsParam = Expression.Parameter(typeof(ElapsedEventArgs));
                var call = Expression.Call(Expression.Constant(Component), customHandler, senderParam, eventArgsParam);
                _customHandler = Expression.Lambda<Action<object, ElapsedEventArgs>>(call, senderParam, eventArgsParam).Compile();
            }

            _timer.Interval = Interval.TotalMilliseconds;
            _timer.Elapsed += Timer_Elapsed;
        }

        /// <summary>
        /// Starts the timer on first render.
        /// </summary>
        /// <param name="firstRender"></param>
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);

            if (firstRender)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        protected override void OnComponentDisposed()
        {
            base.OnComponentDisposed();

            _timer.Stop();
            _timer.Elapsed -= Timer_Elapsed;
            _timer.Dispose();
        }

        private void Timer_Elapsed(object sender, ElapsedEventArgs e)
        {
            if (_customHandler != null)
                _customHandler(sender, e);
            else
                Component.OnTimerElapsedHandler(sender, e);
        }
    }
}
