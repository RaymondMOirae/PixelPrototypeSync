using System.Threading.Tasks;

namespace Prototype.UI
{
    public interface IUIPanel
    {
        void Show(float transitionTime);
        void Hide(float transitionTime);
        
        Task ShowAsync(float transitionTime);
        Task HideAsync(float transitionTime);
        
    }
}