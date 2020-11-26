using System.Threading.Tasks;

namespace Prototype.UI
{
    public interface IUIPanel
    {
        void Show(float transitionTime);
        void Hide(float transitionTime);
        
        Task ShowAsync(float transitionTime);
        Task HideAsync(float transitionTime);

        Task ShowAndWaitClose(float transitionTime);

        Task ShowPopup(IUIPanel panel);
    }
}