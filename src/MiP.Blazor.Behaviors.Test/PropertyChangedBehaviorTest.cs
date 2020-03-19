using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;

namespace MiP.Blazor.Behaviors.Test
{
    [TestClass]
    public class PropertyChangedBehaviorTest
    {
        private TestModel _testModel;
        private IBehavior _propertyChangedBehavior;

        [TestInitialize]
        public void Initialize()
        {
            _testModel = new TestModel();
            _propertyChangedBehavior = new PropertyChangedBehavior();
        }

        [TestMethod]
        public void Changing_property_triggers_rerender()
        {
            // arrange
            var stateHasChangedWasCalled = false;

            var component = new TestableBehaviorComponent(_testModel, _propertyChangedBehavior)
            {
                PropertyChanged = () => stateHasChangedWasCalled = true,
            };

            // simulate initializing by blazor
            _propertyChangedBehavior.Component = component;
            _propertyChangedBehavior.OnInitialized();
            _propertyChangedBehavior.OnParametersSet();

            // act
            _testModel.Data = 13;

            // assert
            stateHasChangedWasCalled.Should().BeTrue();
        }

        [TestMethod]
        public void Unsubscribes_when_disposed()
        {
            // arrange
            var stateHasChangedWasCalled = false;

            var component = new TestableBehaviorComponent(_testModel, _propertyChangedBehavior)
            {
                PropertyChanged = () => stateHasChangedWasCalled = true,
            };

            // simulate initializing by blazor
            _propertyChangedBehavior.Component = component;
            _propertyChangedBehavior.OnInitialized();
            _propertyChangedBehavior.OnParametersSet();

            _testModel.Data = 13;
            Assert.IsTrue(stateHasChangedWasCalled);
            stateHasChangedWasCalled = false;

            // simulate removing from UI by blazor
            _propertyChangedBehavior.ComponentDisposed();

            // act
            _testModel.Data = 13;

            // assert
            stateHasChangedWasCalled.Should().BeFalse();
        }

        private class TestableBehaviorComponent : BehaviorComponent
        {
            [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
            private IBehavior PropertyChangedBehavior { get; }
            [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
            private TestModel TestModel { get; }

            public Action PropertyChanged { get; set; }

            public TestableBehaviorComponent(TestModel testModel, IBehavior propertyChangedBehavior)
            {
                TestModel = testModel;
                PropertyChangedBehavior = propertyChangedBehavior;
            }

            protected internal override void OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)
            {
                PropertyChanged();
            }
        }

        private class TestModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            private int _data;

            public int Data
            {
                get { return _data; }
                set
                {
                    _data = value;
                    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Data)));
                }
            }
        }
    }
}
