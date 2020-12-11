using UnityEngine;

namespace Prototype.Inventory
{
    public abstract class ItemType : ScriptableObject
    {
        public int ItemID;
        public string ItemName;
        public abstract Sprite DroppedSprite { get; }
        public abstract Sprite InventoryIcon { get; }
        public abstract Sprite InventoryBackground { get; }
        
        public abstract string Properties { get; }
        public abstract string Description { get; }
        
    }
}