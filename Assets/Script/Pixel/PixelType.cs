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

        public float weight = 1;
        
        public float endurance = 1;
        
        public float hardness = 1;

        public float droppedSize = 0.2f;

        public PixelAttribute Attribute;

        [TextArea] [SerializeField] private string description = "Descriptions";

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

        public override string Properties =>
            $"Hardness: {hardness}\r\n" +
            $"Weight: {weight}\r\n" +
            $"Endurance: {endurance}\r\n";

        public override string Description => description;

        void GenerateDroppedSprite()
        {
            var pixelsPerUnit = sprite.pixelsPerUnit * sprite.bounds.size.x / droppedSize;
            _droppedSprite = Sprite.Create(sprite.texture, sprite.rect, Vector2.one / 2, pixelsPerUnit);
        }
    }
}