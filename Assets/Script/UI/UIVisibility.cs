using System;
using System.Threading.Tasks;
using UnityEngine;

namespace Prototype.UI
{
    [ExecuteInEditMode]
    [ExecuteAlways]
    [RequireComponent(typeof(CanvasGroup))]
    public class UIVisibility : MonoBehaviour
    {
        [SerializeField] private float TransitionTime = .2f;
        [SerializeField] private bool ShowOnLoad = false;
        
        [SerializeField]
        private bool VisibleInEditor = true;

        private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
            if (Application.isPlaying)
            {
                if(ShowOnLoad)
                    Show();
                else
                {
                    _canvasGroup.alpha = 0;
                    gameObject.SetActive(false);
                }
            }
        }

        private void Update()
        {
            if (!Application.isPlaying)
            {
                _canvasGroup.alpha = VisibleInEditor ? 1 : 0;
                _canvasGroup.interactable = VisibleInEditor ? true : false;
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
            Debug.Log(gameObject.name);
            StopAllCoroutines();
            StartCoroutine(Utility.HideUI(_canvasGroup, TransitionTime, true));
        }
        
        public async Task ShowAsync()
        {
            await Utility.ShowUIAsync(_canvasGroup, TransitionTime);
        }

        public async Task HideAsync()
        {
            await Utility.HideUIAsync(_canvasGroup, TransitionTime);
        }

        public void Lock()
        {
            _canvasGroup.interactable = false;
            _canvasGroup.blocksRaycasts = false;
        }

        public void Unlock()
        {
            _canvasGroup.interactable = true;
            _canvasGroup.blocksRaycasts = true;
        }
        
    }
}