using System;
using System.Threading.Tasks;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.UI
{
    public class PopupMessageManager : Singleton<PopupMessageManager>
    {
        [SerializeField] private RectTransform Container;

        private void Reset()
        {
            Container = transform as RectTransform;
        }

        public async void Show(string message)
        {
            var msgUI = GameObjectPool.Get<PopupMessage>(UISettings.Current.PopupMessagePrefab);
            msgUI.UpdateUI(message);
            msgUI.transform.SetParent(Container);
            await msgUI.Show();
            GameObjectPool.Release(UISettings.Current.PopupMessagePrefab, msgUI);
        }
    }
}