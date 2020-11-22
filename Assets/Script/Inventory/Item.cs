using System;

namespace Prototype.Inventory
{
    public abstract class Item
    {
        public abstract ItemType ItemType { get; }
    }
}