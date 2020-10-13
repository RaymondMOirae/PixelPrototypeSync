using UnityEngine;

namespace Prototype.Element
{
    [CreateAssetMenu(fileName = "Pixel", menuName = "Prototype/Pixel")]
    public class PixelElement : ScriptableObject
    {
        public Sprite sprite;
        
        public new string name;

        public float weight = 1;
        
        public float hardness = 1;
        
        public float damage = 1;
        
        public float defence = 1;

    }
}