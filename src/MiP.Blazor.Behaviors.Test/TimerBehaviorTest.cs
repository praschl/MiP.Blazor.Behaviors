using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;
using System.Timers;

namespace MiP.Blazor.Behaviors.Test
{
    [TestClass]
    public class TimerBehaviorTest
    {
        [TestMethod]
        public async Task Timer_calls_TimerElapsed_from_component()
        {
            var numberOfCallsToTimerElapsed = 0;

            var component = new TestableTimerComponent { TimerElapsed = () => numberOfCallsToTimerElapsed++ };

            component.CallOnInitialized();
            component.CallOnAfterRender(true);

            await Task.Delay(250);

            numberOfCallsToTimerElapsed.Should().Be(2);
        }

        [TestMethod]
        public async Task Timer_calls_Custom_TimerElapsed_from_component()
        {
            var numberOfCallsToTimerElapsed = 0;

            var component = new TestableTimerComponent { CustomTimerElapsed = () => numberOfCallsToTimerElapsed++ };

            component.CallOnInitialized();
            component.CallOnAfterRender(true);

            await Task.Delay(250);

            numberOfCallsToTimerElapsed.Should().Be(2);
        }

        [TestMethod]
        public async Task Timer_stops_when_disposed()
        {
            var numberOfCallsToTimerElapsed = 0;

            var component = new TestableTimerComponent { TimerElapsed = () => numberOfCallsToTimerElapsed++ };

            component.CallOnInitialized();
            component.CallOnAfterRender(true);

            await Task.Delay(150);

            component.Dispose();

            await Task.Delay(200);

            numberOfCallsToTimerElapsed.Should().Be(1);
        }

        [TestMethod]
        public async Task Timer_does_not_start_on_subsequent_renders()
        {
            var numberOfCallsToTimerElapsed = 0;

            var component = new TestableTimerComponent { TimerElapsed = () => numberOfCallsToTimerElapsed++ };

            component.CallOnInitialized();
            component.CallOnAfterRender(false);

            await Task.Delay(150);

            numberOfCallsToTimerElapsed.Should().Be(0);
        }

        private class TestableTimerComponent : BehaviorComponent
        {
            public TimerBehavior Behavior1 { get; } = new TimerBehavior { Interval = TimeSpan.FromSeconds(0.1) };
            public TimerBehavior Behavior2 { get; } = new TimerBehavior { Interval = TimeSpan.FromSeconds(0.1) };

            public Action TimerElapsed { get; set; }
            public Action CustomTimerElapsed { get; set; }

            public void CallOnInitialized()
            {
                OnInitialized();
            }

            public void CallOnAfterRender(bool firstRender)
            {
                base.OnAfterRender(firstRender);
            }

            protected internal override void OnTimerElapsedHandler(object sender, ElapsedEventArgs e)
            {
                TimerElapsed?.Invoke();
            }

            public void Behavior2_TimerElapsedHandler(object sender, ElapsedEventArgs e)
            {
                CustomTimerElapsed?.Invoke();
            }
        }
    }
}
