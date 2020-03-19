using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace MiP.Blazor.Behaviors.Test
{
    [TestClass]
    public class BehaviorTest
    {
        [TestMethod]
        public void OnInitialized_sets_component()
        {
            var component = A.Fake<IDisposable>();

            var behavior = new TestableBehavior
            {
                WhenOnInitialized = () => { }
            };

            ((IBehavior)behavior).Component = component;
            ((IBehavior)behavior).OnInitialized();

            behavior.GetComponent().Should().BeSameAs(component);
        }

        [TestMethod]
        public void OnInitialized_forwards_call()
        {
            var component = A.Fake<IDisposable>();

            bool called = false;

            var behavior = new TestableBehavior
            {
                WhenOnInitialized = ()=> called = true
            };

            ((IBehavior)behavior).Component = component;
            ((IBehavior)behavior).OnInitialized();

            called.Should().BeTrue();
        }

        [TestMethod]
        public void OnInitializedAsync_forwards_call()
        {
            var component = A.Fake<IDisposable>();

            bool called = false;

            var behavior = new TestableBehavior
            {
#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
                WhenOnInitializedAsync = async () => called = true
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
            };

            ((IBehavior)behavior).OnInitializedAsync();

            called.Should().BeTrue();
        }

        [TestMethod]
        public void Instance_must_not_be_shared_between_components()
        {
            // arrange
            var component1 = A.Fake<IDisposable>();
            var component2 = A.Fake<IDisposable>();

            var behavior = new TestableBehavior();

            ((IBehavior)behavior).Component = component1;

            // act
            Action reuseBehavior = () => ((IBehavior)behavior).Component = component2;

            // assert
            reuseBehavior.Should().Throw<InvalidOperationException>().WithMessage("An behaviors instance must not be shared between components. If you injected the behavior, register it with transient lifetime.");
        }

        private class TestableBehavior : Behavior<IDisposable>
        {
            public IDisposable GetComponent() => Component;

            public Action WhenOnInitialized { get; set; }
            public Func<Task> WhenOnInitializedAsync { get;set;}

            protected override void OnInitialized()
            {
                WhenOnInitialized();
            }

            protected async override Task OnInitializedAsync()
            {
                await WhenOnInitializedAsync().ConfigureAwait(false);
            }
        }
    }
}
