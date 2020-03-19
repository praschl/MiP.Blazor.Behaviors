using System.Diagnostics;

namespace MiP.Blazor.Behaviors.Example.Data
{
    public class LogBehavior : Behavior
    {
        protected override void OnParametersSet()
        {
            Debug.WriteLine($"{Component.GetType().FullName}: OnAfterParametersSet()");
        }
    }
}
