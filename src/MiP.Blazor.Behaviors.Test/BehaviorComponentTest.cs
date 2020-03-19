using FakeItEasy;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;

namespace MiP.Blazor.Behaviors.Test
{
    [TestClass]
    public class BehaviorComponentTest
    {
        private TestableBehaviorComponent _component;
        private IBehavior _first;
        private IBehavior _second;

        [TestInitialize]
        public void Initialize()
        {
            _first = A.Fake<IBehavior>();
            _second = A.Fake<IBehavior>();

            _component = new TestableBehaviorComponent(_first, _second);
            _component.CallOnInitialized();
        }

        [TestMethod]
        public void Component_is_initialized()
        {
            _first.Component.Should().BeSameAs(_component);
            _second.Component.Should().BeSameAs(_component);
        }

        [TestMethod]
        public void MemberName_is_initialized()
        {
            _first.MemberName.Should().Be(nameof(TestableBehaviorComponent.First));
            _second.MemberName.Should().Be(nameof(TestableBehaviorComponent.Second));
        }

        [TestMethod]
        public void OnInitialized_forwards_to_all_Behaviors()
        {
            A.CallTo(() => _first.OnInitialized()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _second.OnInitialized()).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task OnInitializedAsync_forwards_to_all_Behaviors()
        {
            await _component.CallOnInitializedAsync();

            A.CallTo(() => _first.OnInitializedAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _second.OnInitializedAsync()).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void OnParametersSet_forwards_to_all_Behaviors()
        {
            _component.CallOnParametersSet();

            A.CallTo(() => _first.OnParametersSet()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _second.OnParametersSet()).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public async Task OnParametersSetAsync_forwards_to_all_Behaviors()
        {
            await _component.CallOnParametersSetAsync();

            A.CallTo(() => _first.OnParametersSetAsync()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _second.OnParametersSetAsync()).MustHaveHappenedOnceExactly();
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public void OnAfterRender_forwards_to_all_Behaviors(bool firstRender)
        {
            _component.CallOnAfterRender(firstRender);

            A.CallTo(() => _first.OnAfterRender(firstRender)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _second.OnAfterRender(firstRender)).MustHaveHappenedOnceExactly();
        }

        [DataRow(false)]
        [DataRow(true)]
        [DataTestMethod]
        public async Task OnAfterRenderAsync_forwards_to_all_Behaviors(bool firstRender)
        {
            await _component.CallOnAfterRenderAsync(firstRender);

            A.CallTo(() => _first.OnAfterRenderAsync(firstRender)).MustHaveHappenedOnceExactly();
            A.CallTo(() => _second.OnAfterRenderAsync(firstRender)).MustHaveHappenedOnceExactly();
        }

        [TestMethod]
        public void OnDispose_forwards_to_all_Behaviors()
        {
            _component.CallDispose();

            A.CallTo(() => _first.ComponentDisposed()).MustHaveHappenedOnceExactly();
            A.CallTo(() => _second.ComponentDisposed()).MustHaveHappenedOnceExactly();
        }

        private class TestableBehaviorComponent : BehaviorComponent
        {
            public TestableBehaviorComponent(IBehavior first, IBehavior second)
            {
                First = first;
                Second = second;
            }

            public IBehavior First { get; }
            public IBehavior Second { get; }

            public void CallOnInitialized()
            {
                OnInitialized();
            }

            public Task CallOnInitializedAsync()
            {
                return OnInitializedAsync();
            }

            public void CallOnParametersSet()
            {
                OnParametersSet();
            }

            public Task CallOnParametersSetAsync()
            {
                return OnParametersSetAsync();
            }

            public void CallOnAfterRender(bool firstRender)
            {
                OnAfterRender(firstRender);
            }

            public Task CallOnAfterRenderAsync(bool firstRender)
            {
                return OnAfterRenderAsync(firstRender);
            }

            public void CallDispose()
            {
                Dispose(true);
            }
        }
    }
}
