using UnityEngine;

namespace Prototype.Inventory
{
    public abstract class ItemType : ScriptableObject
    {
        public int ItemID;
        public string ItemName;
        public abstract Sprite Image { get; }
        
        public abstract string Properties { get; }
        public abstract string Description { get; }
        
    }
}