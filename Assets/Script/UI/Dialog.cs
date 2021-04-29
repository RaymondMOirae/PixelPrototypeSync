using System;
using System.Threading.Tasks;
using Prototype.Utils;
using UnityEngine;
using UnityEngine.UI;

namespace Prototype.UI
{
    [Flags]
    public enum DialogType : int
    {
        Ok = 0b1,
        Cancel = 0b10,
        OkCancel = Ok | Cancel
    }
    public class Dialog : UIPanel
    {
        public DialogType Type;
        [SerializeField] private Button ButtonOk;
        [SerializeField] private Button ButtonCancel;
        [SerializeField] private Text Text;
        private TaskCompletionSource<bool> _resultPromise;
        private TaskCompletionSource<bool> _closePromise;
        private bool _active = false;

        protected override void Awake()
        {
            base.Awake();

            if (ButtonOk)
            {
                ButtonOk.onClick.AddListener(Ok);
            }

            if (ButtonCancel)
            {
                ButtonCancel.onClick.AddListener(Cancel);
            }
        }

        private async Task ShowAndWaitFullyClose(string text, DialogType dialogType, TaskCompletionSource<bool> completionSource)
        {
            if (_active)
                throw new Exception("Dialog already active.");
            _active = true;
            
            _resultPromise = completionSource;
            _closePromise = new TaskCompletionSource<bool>();
        
            OverlayUIManager.Instance.AddDialog(this);
            (transform as RectTransform).anchoredPosition = Vector2.zero;
            ButtonOk.gameObject.SetActive((dialogType & DialogType.Ok) == DialogType.Ok);
            ButtonCancel.gameObject.SetActive((dialogType & DialogType.Cancel) == DialogType.Cancel);
            
            Show();
            await _closePromise.Task;
        }

        private static async void WaitAndRelease(GameObject prefab, Dialog dialog, Task task)
        {
            await task;
            GameObjectPool.Release(prefab, dialog);
        }

        public static async Task<bool> ShowPrefabAsync(GameObject prefab, string text, UIPanel owner, DialogType dialogType = DialogType.Ok)
        {
            var dialog = GameObjectPool.Get<Dialog>(prefab);
            if (!dialog)
                throw new Exception("Prefab dose not have a component of Dialog.");

            dialog.Text.text = text;
            owner.ShowPopup(dialog);
            
            var resultPromise = new TaskCompletionSource<bool>();
            WaitAndRelease(prefab, dialog, dialog.ShowAndWaitFullyClose(text, dialogType, resultPromise));
            var result = await resultPromise.Task;
            
            owner.ClosePopup(dialog);
            return result;
        }

        async void Ok()
        {
            if (!_active)
                return;
            _active = false;
            
            
            _resultPromise?.SetResult(true);
            await HideAsync();
            _closePromise.SetResult(true);
        }
        
        async void Cancel()
        {
            if (!_active)
                return;
            _active = false;
            
            _resultPromise?.SetResult(false);
            await HideAsync();
            _closePromise.SetResult(true);
        }
    }
}