using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Inventory
{
    [Serializable]
    public class ItemGroup : List<Item>
    {
        [SerializeField] private readonly ItemType _itemType;
        public ItemType ItemType => _itemType;

        public ItemGroup(ItemType type)
        {
            _itemType = type;
        }
        
        public new void Add(Item item)
        {
            if (ItemType != item.ItemType)
                throw new Exception($"Cannot add item of type '{item.ItemType.name}' into group of '{ItemType.name}'");
            base.Add(item);
        }

        public T Take<T>() where T : Item
        {
            if (Count <= 0)
                return null;
            return this.PopBack() as T;
        }

        public T Take<T>(T item) where T : Item
        {
            if(item.ItemType != ItemType)
                throw new Exception($"Item type mismatch.");
            var idx = IndexOf(item);
            if(idx < 0)
                throw new Exception($"Item not found in group.");
            Remove(item);
            return item;
        }

        public static implicit operator bool(ItemGroup itemGroup)
            => !(itemGroup is null) && itemGroup.ItemType;
    }
}