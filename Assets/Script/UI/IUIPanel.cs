using System.Threading.Tasks;

namespace Prototype.UI
{
    public interface IUIPanel
    {
        void Show();
        void Hide();
        
        Task ShowAsync();
        Task HideAsync();

        Task ShowAndWaitClose();

        void ShowPopup(IUIPanel panel);

        void ClosePopup(IUIPanel panel);
    }
}