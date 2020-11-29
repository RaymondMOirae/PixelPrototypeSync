using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour, IUIPanel
    {
        [SerializeField] private float TransitionTime = .2f;
        [SerializeField] private bool ShowOnLoad = false;
        private CanvasGroup _canvasGroup;
        private TaskCompletionSource<int> _completionSource;
        private IUIPanel _overlayPopup;
        protected virtual void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if(ShowOnLoad)
                Show();
            else
            {
                _canvasGroup.alpha = 0;
                gameObject.SetActive(false);
            }
        }

        public void Show()
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Utility.ShowUI(_canvasGroup, TransitionTime));
        }

        public void Hide()
        {
            StopAllCoroutines();
            NotifyUIHide();
            StartCoroutine(Utility.HideUI(_canvasGroup, TransitionTime, true));
        }

        void NotifyUIHide()
        {
            if (_completionSource.NotNull())
            {
                _completionSource.SetResult(0);
                _completionSource = null;
            }
        }

        public async Task ShowAsync()
        {
            await Utility.ShowUIAsync(_canvasGroup, TransitionTime);
        }

        public async Task HideAsync()
        {
            NotifyUIHide();
            await Utility.HideUIAsync(_canvasGroup, TransitionTime);
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
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
            _overlayPopup = uiPanel;
        }

        public void ClosePopup(IUIPanel panel)
        {
            if(panel != _overlayPopup)
                throw new Exception("Popup instance mismatch.");
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
            _overlayPopup = null;
        }
    }
}