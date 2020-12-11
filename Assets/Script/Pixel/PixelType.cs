using Prototype.Inventory;
using UnityEngine;

namespace Prototype.Element
{
    public enum PixelAttribute
    {
        Default,
        Pivot,
        LightSource,
        Hand,
    }
    
    [CreateAssetMenu(fileName = "Pixel", menuName = "Prototype/Pixel")]
    public class PixelType : ItemType
    {
        public Sprite sprite;
        private Sprite _droppedSprite;
        
        public new string name;

        public float weight = 1;
        
        public float hardness = 1;
        
        public float damage = 1;
        
        public float defence = 1;

        public float droppedSize = 0.2f;

        public PixelAttribute Attribute;

        public override Sprite InventoryIcon => sprite;
        public override Sprite InventoryBackground => null;

        public override Sprite DroppedSprite
        {
            get
            {
                if (_droppedSprite)
                    return _droppedSprite;
                GenerateDroppedSprite();
                return _droppedSprite;
            }
        }
        public override string Properties => "Properties";

        public override string Description
        {
            get => "Description";
        }

        void GenerateDroppedSprite()
        {
            var pixelsPerUnit = sprite.pixelsPerUnit * sprite.bounds.size.x / droppedSize;
            _droppedSprite = Sprite.Create(sprite.texture, sprite.rect, Vector2.one / 2, pixelsPerUnit);
        }
    }
}