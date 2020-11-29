using System;
using System.Threading.Tasks;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.UI
{
    public class OverlayUIManager : Singleton<OverlayUIManager>
    {
        [SerializeField] private RectTransform PopupMessageContainer;
        [SerializeField] private RectTransform DialogContainer;

        private void Reset()
        {
            PopupMessageContainer = transform as RectTransform;
        }

        public async void ShowPopupMessage(string message)
        {
            var msgUI = GameObjectPool.Get<PopupMessage>(UISettings.Current.PopupMessagePrefab);
            msgUI.UpdateUI(message);
            msgUI.transform.SetParent(PopupMessageContainer);
            await msgUI.Show();
            GameObjectPool.Release(UISettings.Current.PopupMessagePrefab, msgUI);
        }

        public void AddDialog(Dialog dialog)
        {
            dialog.transform.SetParent(DialogContainer);
        }
        
    }
}