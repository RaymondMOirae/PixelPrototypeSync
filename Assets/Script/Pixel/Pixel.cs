using UnityEngine;

namespace Prototype.Element
{
    public class Pixel
    {
        public PixelType Type { get; }
        public float Hardness;

        public Pixel(PixelType type)
        {
            this.Type = type;
            this.Hardness = type.hardness;
        }
    }
}