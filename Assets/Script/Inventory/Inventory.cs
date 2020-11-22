using System.Collections.Generic;
using Prototype.Element;

namespace Prototype.Inventory
{
    public class Inventory
    {
        public readonly List<ItemGroup> ItemGroups = new List<ItemGroup>();

        public void SaveItem(Item item)
        {
            if (item is null)
                return;
            foreach (var group in ItemGroups)
            {
                if (group.ItemType == item.ItemType)
                {
                    group.Add(item);
                    return;
                }
            }

            var newGroup = new ItemGroup(item.ItemType);
            newGroup.Add(item);
            ItemGroups.Add(newGroup);
        }

        public T Take<T>(T item) where T : Item 
            => Take(item) as T;

        public Item Take(Item item)
        {
            foreach (var group in ItemGroups)
            {
                if (group.ItemType == item.ItemType)
                    return group.Take(item);
            }

            return null;
        }

        public T Take<T>(ItemType itemType) where T : Item
        {
            foreach (var group in ItemGroups)
            {
                if (group.ItemType == itemType)
                    return group.Take<T>();
            }

            return null;
        }

        public Item Take(ItemType itemType)
            => Take<Item>(itemType);

    }
}