using Prototype.Inventory;
using Prototype.Settings;
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
        public override Sprite InventoryIcon => _image;

        public override Sprite InventoryBackground
        {
            get
            {
                if (Size.x == Size.y && Size.x == 8)
                    return UISettings.Current.InventoryPreviewBackgrounds.Image8x8;
                if (Size.x == Size.y && Size.x == 12)
                    return UISettings.Current.InventoryPreviewBackgrounds.Image12x12;
                if (Size.x == Size.y && Size.x == 16)
                    return UISettings.Current.InventoryPreviewBackgrounds.Image16x16;
                return null;
            }
        }

        public override Sprite DroppedSprite => _image;

        public void Init(PixelImage pixelImage)
        {
            Size = pixelImage.Size;
            _properties = $"Size: {pixelImage.Size.x} × {pixelImage.Size.y}";
            _description = $"A {pixelImage.Size.x} × {pixelImage.Size.y} weapon template. Use to create a new weapon.";
            _image = Sprite.Create(pixelImage.Texture,
                new Rect(0, 0, pixelImage.Texture.width, pixelImage.Texture.height), Vector2.one / 2);
        }
    }
}