using Prototype.Inventory;
using UnityEngine;

namespace Prototype.Element
{
    /// <summary>
    /// Virtual ItemType, used to distinguish PixelImage item.
    /// Should create a new instance for each PixelImage item.
    /// </summary>
    public class PixelImageType : ItemType
    {
        public Vector2Int Size;
        private string _properties;
        private string _description;
        public override string Properties => _properties;
        public override string Description => _description;

        public void Init(Vector2Int size)
        {
            _properties = $"Size: {size.x} × {size.y}";
            _description = $"A {size.x} × {size.y} weapon template. Use to create a new weapon.";
        }
    }
}