using System;
using Prototype.UI.Inventory;
using UnityEngine;

namespace Prototype.Settings
{
    [Serializable]
    public class UISettings : SettingEntry<UISettings>
    {
        public GameObject InventorySlotPrefab;
    }
}