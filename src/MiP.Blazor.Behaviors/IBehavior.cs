using System.Threading.Tasks;

namespace MiP.Blazor.Behaviors
{
    internal interface IBehavior
    {
        object Component { get; set; }
        string MemberName { get; set; }

        void OnInitialized();
        Task OnInitializedAsync();

        void OnParametersSet();
        Task OnParametersSetAsync();

        void OnAfterRender(bool firstRender);
        Task OnAfterRenderAsync(bool firstRender);

        void ComponentDisposed();
    }
}
