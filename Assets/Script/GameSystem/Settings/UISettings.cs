using System;
using Prototype.UI.Inventory;
using UnityEngine;

namespace Prototype.Settings
{
    [Serializable]
    public class UISettings : SettingEntry<UISettings>
    {
        public GameObject InventorySlotPrefab;
        public GameObject PopupMessagePrefab;
        public GameObject BasicDialogPrefab;

        public InventoryPreviewBackgrounds_T InventoryPreviewBackgrounds;

        [Serializable]
        public struct InventoryPreviewBackgrounds_T
        {
            public Sprite Image8x8;
        }
    }
}