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
        
        private Sprite _image;
        public override Sprite Image => _image;

        public void Init(PixelImage pixelImage)
        {
            _properties = $"Size: {pixelImage.Size.x} × {pixelImage.Size.y}";
            _description = $"A {pixelImage.Size.x} × {pixelImage.Size.y} weapon template. Use to create a new weapon.";
            _image = Sprite.Create(pixelImage.Texture,
                new Rect(0, 0, pixelImage.Texture.width, pixelImage.Texture.height), Vector2.one / 2);
        }
    }
}