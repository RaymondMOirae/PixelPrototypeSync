using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class UIPanel : MonoBehaviour, IUIPanel
    {
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

        public void Show(float transitionTime = .2f)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Utility.ShowUI(_canvasGroup, transitionTime));
        }

        public void Hide(float transitionTime = .2f)
        {
            StopAllCoroutines();
            NotifyUIHide();
            StartCoroutine(Utility.HideUI(_canvasGroup, transitionTime, true));
        }

        void NotifyUIHide()
        {
            if (_completionSource.NotNull())
            {
                _completionSource.SetResult(0);
                _completionSource = null;
            }
        }

        public async Task ShowAsync(float transitionTime = .2f)
        {
            await Utility.ShowUIAsync(_canvasGroup, transitionTime);
        }

        public async Task HideAsync(float transitionTime = .2f)
        {
            NotifyUIHide();
            await Utility.HideUIAsync(_canvasGroup, transitionTime);
        }

        public async Task ShowAndWaitClose(float transitionTime = .2f)
        {
            _completionSource = new TaskCompletionSource<int>();
            Show(transitionTime);
            await _completionSource.Task;
        }

        public async Task ShowPopup(IUIPanel uiPanel)
        {
            if(_overlayPopup.NotNull())
                throw new Exception("A popup is still on top of current UI.");
            _canvasGroup.interactable = false;
            _overlayPopup = uiPanel;
            await uiPanel.ShowAndWaitClose(.2f);
            _canvasGroup.interactable = true;
            _overlayPopup = null;
        }
    }
}