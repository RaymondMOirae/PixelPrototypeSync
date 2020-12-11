using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Inventory;
using Prototype.Settings;
using Prototype.Utils;

namespace Prototype.UI.Inventory
{
    public class InventoryPanel : SelectGroup<ItemGroup>
    {
        public void LoadInventory(Prototype.Inventory.Inventory inventory, Func<ItemType, bool> itemFilter = null)
        {
            itemFilter = itemFilter ?? DefaultItemFIlter;
            
            Unload();
            foreach (var group in inventory.ItemGroups.Where(group => itemFilter(group.ItemType)))
            {
                var slot = GameObjectPool.Get<InventorySlot>(UISettings.Current.InventorySlotPrefab);
                slot.UpdateUI(group);
                AddItem(group, slot);
            }
        }

        static bool DefaultItemFIlter(ItemType itemType)
        {
            return true;
        }
        

        void Unload()
        {
            ClearItem(item => GameObjectPool.Release(UISettings.Current.InventorySlotPrefab, item));
        }
    }
}