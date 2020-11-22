using System.Threading.Tasks;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class GlobalUIPanel<T> : Singleton<T>, IUIPanel where T : GlobalUIPanel<T>
    {
        private CanvasGroup canvasGroup;

        protected override void Awake()
        {
            base.Awake();

            canvasGroup = GetComponent<CanvasGroup>();
        }

        public void Show(float transitionTime = .2f)
        {
            gameObject.SetActive(true);
            StopAllCoroutines();
            StartCoroutine(Utility.ShowUI(canvasGroup, transitionTime));
        }

        public Task ShowAsync(float time = .2f)
        {
            return Utility.ShowUIAsync(canvasGroup, time);
        }

        public void Hide(float time = .2f)
        {
            StopAllCoroutines();
            StartCoroutine(Utility.HideUI(canvasGroup, time, true));
        }

        public Task HideAsync(float time = .2f)
        {
            return Utility.HideUIAsync(canvasGroup, time);
        }
    }
}