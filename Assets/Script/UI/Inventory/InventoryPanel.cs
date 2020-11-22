using System.Collections.Generic;
using Prototype.Inventory;
using Prototype.Settings;
using Prototype.Utils;

namespace Prototype.UI.Inventory
{
    public class InventoryPanel : SelectGroup<ItemGroup>
    {
        public void UpdateUI(IEnumerable<ItemGroup> itemGroups)
        {
            foreach (var group in itemGroups)
            {
                var slot = GameObjectPool.Get<InventorySlot>(UISettings.Current.InventorySlotPrefab);
                slot.UpdateUI(group);
                AddItem(group, slot);
            }
        }
    }
}