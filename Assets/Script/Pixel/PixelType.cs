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
        
        public new string name;

        public float weight = 1;
        
        public float hardness = 1;
        
        public float damage = 1;
        
        public float defence = 1;

        public PixelAttribute Attribute;

        public override string Properties => "Properties";

        public override string Description
        {
            get => "Description";
        }
    }
}