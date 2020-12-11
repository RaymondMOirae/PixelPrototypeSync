using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Inventory
{
    [Serializable]
    public class Inventory
    {
        [SerializeField] private List<ItemGroup> _itemGroups = new List<ItemGroup>();
        public IReadOnlyList<ItemGroup> ItemGroups => _itemGroups;

        public void SaveItem(Item item)
        {
            if (item is null)
                return;
            foreach (var group in _itemGroups)
            {
                if (group.ItemType == item.ItemType)
                {
                    group.Add(item);
                    return;
                }
            }

            var newGroup = new ItemGroup(item.ItemType);
            newGroup.Add(item);
            _itemGroups.Add(newGroup);
        }

        public T Take<T>(T item) where T : Item 
            => Take(item) as T;

        public Item Take(Item item)
        {
            foreach (var group in _itemGroups)
            {
                if (group.ItemType == item.ItemType)
                    return group.Take(item);
            }

            return null;
        }

        public T Take<T>(ItemType itemType) where T : Item
        {
            foreach (var group in _itemGroups)
            {
                if (group.ItemType == itemType)
                    return group.Take<T>();
            }

            return null;
        }

        public Item Take(ItemType itemType)
            => Take<Item>(itemType);

        public DroppedItem DropOne(ItemType itemType, Vector2 position)
        {
            var item = Take(itemType);
            if (item is null)
                return null;

            var droppedItem = GameObjectPool.Get<DroppedItem>(GamePrefabs.Current.DroppedItem);
            droppedItem.Init(item);
            droppedItem.DropAt(position);
            return droppedItem;
        }

        public Item PickUp(DroppedItem droppedItem)
        {
            var item = droppedItem.Item;
            
            GameObjectPool.Release(GamePrefabs.Current.DroppedItem, droppedItem);
            
            SaveItem(item);
            return item;
        }
    }
}