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
        private TestModel _testModel1;
        private TestModel _testModel2;
        private IBehavior _propertyChangedBehavior;

        private int _eventCount = 0;
        private TestableBehaviorComponent _component;

        [TestInitialize]
        public void Initialize()
        {
            _testModel1 = new TestModel();
            _testModel2 = new TestModel();
            _propertyChangedBehavior = new PropertyChangedBehavior();

            _component = new TestableBehaviorComponent(_propertyChangedBehavior)
            {
                PropertyChanged = () => _eventCount++,
            };

            _component.TestModel1 = _testModel1;

            // simulate initializing by blazor
            _propertyChangedBehavior.Component = _component;
            _propertyChangedBehavior.OnInitialized();
            _propertyChangedBehavior.OnParametersSet();
        }

        [TestMethod]
        public void Changing_property_triggers_rerender()
        {
            // arrange
            // act
            _testModel1.Data = 1;

            // assert
            _eventCount.Should().Be(1);
        }

        [TestMethod]
        public void Unsubscribes_when_disposed()
        {
            // act 1
            _testModel1.Data = 1;

            _eventCount.Should().Be(1);

            // simulate removing from UI by blazor
            _propertyChangedBehavior.ComponentDisposed();

            // act
            _testModel1.Data = 2;

            // assert
            _eventCount.Should().Be(1); // still
        }

        [TestMethod]
        public void Additional_subscribes_after_repeated_OnParametersSet()
        {
            // set a new model
            _component.TestModel1 = new TestModel();
            _component.TestModel2 = new TestModel();

            // act
            _propertyChangedBehavior.OnParametersSet();

            _eventCount.Should().Be(0);

            // two changes after OnParametersSet
            _component.TestModel1.Data = 1;
            _eventCount.Should().Be(1);

            _component.TestModel2.Data = 2;
            _eventCount.Should().Be(2);
        }

        [TestMethod]
        public void Additionally_unsubscribes_after_OnParametersSet()
        {
            // set a new model
            _component.TestModel1 = new TestModel();
            _component.TestModel2 = new TestModel();

            // activate the new model
            _propertyChangedBehavior.OnParametersSet();

            var model1 = _component.TestModel1;
            var model2 = _component.TestModel2;

            // remove models
            _component.TestModel1 = null;
            _component.TestModel2 = null;

            // and call OnParametersSet to unsubscribe from event.
            _propertyChangedBehavior.OnParametersSet();

            _eventCount.Should().Be(0);

            // two changes after OnParametersSet on the old model should not change anything.
            model1.Data = 1;
            _eventCount.Should().Be(0);

            model2.Data = 2;
            _eventCount.Should().Be(0);
        }

        private class TestableBehaviorComponent : BehaviorComponent
        {
            [SuppressMessage("CodeQuality", "IDE0052:Remove unread private members")]
            private IBehavior PropertyChangedBehavior { get; }

            public TestModel TestModel1 { get; set; }
            internal TestModel TestModel2 { get; set; }

            public Action PropertyChanged { get; set; }

            public TestableBehaviorComponent(IBehavior propertyChangedBehavior)
            {
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
