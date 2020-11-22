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
        private void Awake()
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
            StartCoroutine(Utility.HideUI(_canvasGroup, transitionTime, true));
        }

        public async Task ShowAsync(float transitionTime = .2f)
        {
            await Utility.ShowUIAsync(_canvasGroup, transitionTime);
        }

        public async Task HideAsync(float transitionTime = .2f)
        {
            await Utility.HideUIAsync(_canvasGroup, transitionTime);
        }
    }
}