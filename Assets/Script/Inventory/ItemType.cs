using UnityEngine;

namespace Prototype.Inventory
{
    public abstract class ItemType : ScriptableObject
    {
        public int ItemID;
        public Sprite Image;
        public string ItemName;
        
        public abstract string Properties { get; }
        public abstract string Description { get; }
        
    }
}