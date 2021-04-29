using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.UI
{
    [RequireComponent(typeof(UIVisibility))]
    public class UIPanel : MonoBehaviour, IUIPanel
    {
        private UIVisibility _uiVisibility;
        private TaskCompletionSource<int> _completionSource;
        private IUIPanel _overlayPopup;
        protected virtual void Awake()
        {
            _uiVisibility = GetComponent<UIVisibility>();
            if (!_uiVisibility)
                _uiVisibility = gameObject.AddComponent<UIVisibility>();
        }

        public void Show()
        {
            _uiVisibility.Show();
        }

        public void Hide()
        {
            NotifyUIHide();
            _uiVisibility.Hide();
        }

        void NotifyUIHide()
        {
            if (_completionSource.NotNull())
            {
                _completionSource.SetResult(0);
                _completionSource = null;
            }
        }

        public Task ShowAsync()
        {
            return _uiVisibility.ShowAsync();
        }

        public Task HideAsync()
        {
            NotifyUIHide();
            return _uiVisibility.HideAsync();
        }

        public async Task ShowAndWaitClose()
        {
            _completionSource = new TaskCompletionSource<int>();
            Show();
            await _completionSource.Task;
        }

        public void ShowPopup(IUIPanel uiPanel)
        {
            if(_overlayPopup.NotNull())
                throw new Exception("A popup is still on top of current UI.");
            _uiVisibility.Lock();
            _overlayPopup = uiPanel;
        }

        public void ClosePopup(IUIPanel panel)
        {
            if(panel != _overlayPopup)
                throw new Exception("Popup instance mismatch.");
            _uiVisibility.Unlock();
            _overlayPopup = null;
        }
    }
}