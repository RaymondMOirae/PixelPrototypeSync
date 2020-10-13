using UnityEngine;

namespace Prototype.Element
{
    public class Pixel
    {
        public PixelElement element;
        public float hardness;

        public Pixel(PixelElement element)
        {
            this.element = element;
            this.hardness = element.hardness;
        }
    }
}