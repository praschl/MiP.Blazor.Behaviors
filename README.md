# MiP.Blazor.Behaviors
Behaviors for Blazor :-)

## Intention
Changing the behavior of a Razor Component means, you override the Lifetime methods `OnInitialized`, `OnParametersSet`, and/or one of the others. Usually, you could also just derive from `ComponentBase` to create the behavior you want and derive your Razor Component from that new base class.

But what, when you want to use the code of two (or even more) such base components in a Razor Component? You would have to create a new base component and implement both behaviors, leading to to code duplication, because C# would not let you derive from two classes. Even if you extract the code to new classes you would still have to override the lifetime methods and call the code.

## Behaviors to the rescue
MiP.Blazor.Behaviors will let you implement the intended behavior by deriving from a `MiP.Blazor.Behaviors.Behavior<T>`. The behavior can then injected into a Razor Component or it may be instantiated directly and kept in a field. When you derive your Razor Component from `MiP.Blazor.Behaviors.BehaviorComponent`, it will scan for behaviors on the component, and activate them.

You can add multiple behaviors to the razor component and all of them will be active and executing their code. Depending on the behavior, you may also add multiple instances of the same Behavior to the component.

See the example project for more information.

## Getting started
Create a new blazor project and add the `MiP.Blazor.Behaviors` package to it.

```powershell
Install-Package MiP.Blazor.Behaviors
```
Add
```csharp
  services.AddBehavior<PropertyChangedBehavior>();
```
to `ConfigureServices` in `Startup.cs`

Go to the `Counter.razor` file and replace the content with
```csharp 
@page "/Counter"
@using MiP.Blazor.Behaviors
@using System.ComponentModel
@using System.Timers

@* (1) *@
@inherits BehaviorComponent
@* (2) *@
@inject PropertyChangedBehavior propertyChangedBehavior

<p>Random: @_container.Number</p>

@code {
  // (3)
  private TimerBehavior _timerBehavior = new TimerBehavior { Interval = TimeSpan.FromSeconds(1) };

  // (4)
  public class NumberContainer : INotifyPropertyChanged
  {
      public event PropertyChangedEventHandler PropertyChanged;
      private int _number;
      public int Number
      {
          get => _number;
          set
          {
              _number = value;
              PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(Number)));
          }
      }
  }

  // (5)
  private NumberContainer _container = new NumberContainer();

  // (6)
  protected override async void OnTimerElapsedHandler(object sender, ElapsedEventArgs e)
  {
      _container.Number = DateTime.Now.Millisecond;
  }

  // (7)
}
```
When you run the code and go to the counter page, you will notice a random 3 digit number changing every second.

### How this works
At `(1)` we derive from the `BehaviorComponent` which is important, because this base component contains all the logic to discover and execute behaviors.

At `(2)` we inject the `PropertyChangedBehavior`. This behavior scans the current component for properties and fields that support the `INotifyPropertyChanged`, attaches to all the events and invokes StateHasChanged() on the component to force a redraw.

At (3) we also add the `TimerBehavior` with a one second timer. It will call the handler at (6) every second.

(4) is just a class that implements `INotifyPropertyChanged` and has a property that we display, and (5) an instance of this class.

Finally the `OnTimerElapsedHandler` at (6) will update that number, which in turn triggers the event to cause a refresh.

(7) No Dispose? No unattaching from events? No need. The `PropertyChangedBehavior` and the `TimerBehavior` do handle that on their own.

## BehaviorComponent
Components that want to use behaviors must `@inherit BehaviorComponent`. This component contains the logic to enables behaviors. It also implements `IDisposable` and adds a virtual `Dispose(bool)` which calls the `Behavior.OnComponentDisposed`.

## Dependency Injection
Behaviors can be injected using `@inject`. Because an instance of a behavior must not be shared between components, they have to be registered with transient lifetime, which can be done using the `.AddBehavior<TBehavior>()` extension method.

## TimerBehavior
This behavior helps with calling a method at an interval. The behavior handles subscribing and unsubscribing from the `Timer.Elapsed` event for you.

The behavior will by default call the method `OnTimerElapsedHandler(object sender, ElapsedEventArgs e)` which by default does nothing, and can be overridden to suit your needs.

You may on occation want to have two or more Timers in the same component, and have a different handler method for each of them. To have a handler different from the standard handler, just name it after the property that holds the reference to the `TimerBehavior`, for example:
```csharp
@code {
  TimerBehavior _myTimer = new TimerBehavior { Elapsed = TimeSpan.FromSeconds(1) };

  private void _myTimer_TimerElapsedHandler(object sender, ElapsedEventArgs e)
  {
    // ...
  }
}
```
You see the handler starting with `_myTimer` and an additional separating underscore, thats how the `TimerBehavior` will find the correct handler. If no handler with this name is found, the default handler will be used (which may result in no effect at all, since the default implementation does nothing). The parameters must match, too, or it will be ignored.

## PropertyChangedBehavior
Adding this behavior to a component will make the component subscribe to all `INotifyPropertyChanged.PropertyChanged` events of any member of the component that implements `INotifyPropertyChanged`. 

The default handler will call `StateHasChanged` on the component, using `InvokeAsync` to switch to the UI thread. The default behavior can be changed by overriding `OnPropertyChangedHandler(object sender, PropertyChangedEventArgs e)` on the `BehaviorComponent`.

When the component is disposed, the behavior unsubscribes all event handlers.

The behavior uses all instance members whos type implements the `INotifyPropertyChanged` interface. Static members are not supported, but protected and private members are. Nested properties are not searched for `INotifyPropertyChanged`.

## Creating your own Behaviors
To write your own behavior, derive from `Behavior<T>` or `Behavior`.
The `T` may be any Type, for example an interface that is implemented by your component, but any component that uses a behavior, must be assignable to `T`. For `Behavior`, the `T` is `ComponentBase`

Property | Description
--|--
Component | Contains a reference to the component that uses this behavior instance and is typed `T`.
MemberName | Contains the name of the member in the component that holds the reference to this behavior instance.

Since a behavior holds a reference to the component that uses it, each component has to use its own instance. Instances of behaviors must not be shared between components, or unexpected *behavior* will happen.

Override methods to plug into the lifecycle of the component. The names of the methods on the `Behavior` correspond to the lifecycle methods of `ComponentBase`.

*Additional methods are*
Method | Description
--|--
OnComponentDisposed | This method is called, when the component is Disposed.

For an easy example have a look at the `TimerBehavior` or at the `LogBehavior` https://github.com/praschl/MiP.Blazor.Behaviors/blob/master/src/MiP.Blazor.Behaviors.Example/Data/LogBehavior.cs
