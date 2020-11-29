using System;
using System.Threading.Tasks;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI
{
    [RequireComponent(typeof(CanvasGroup))]
    public class PopupMessage : MonoBehaviour
    {
        [SerializeField] private float VisibleTime = 2;
        [SerializeField] private float TransitionTime = .2f;
        [SerializeField] private Text Text;

        private CanvasGroup _canvasGroup;
        
        private void Awake()
        {
            _canvasGroup = GetComponent<CanvasGroup>();
        }

        public void UpdateUI(string msg)
        {
            Text.text = msg;
        }

        public async Task Show()
        {
            await Utility.ShowUIAsync(_canvasGroup, TransitionTime);
            await Task.Delay(Mathf.FloorToInt(VisibleTime * 1000));
            await Utility.HideUIAsync(_canvasGroup, TransitionTime);
        }

        public static void Show(string message)
            => OverlayUIManager.Instance.ShowPopupMessage(message);
    }
}