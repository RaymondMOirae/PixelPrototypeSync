using UnityEngine;

namespace Prototype.Element
{
    public class PixelImage
    {
        public Vector2Int Size { get; }
        public Pixel[,] Pixels { get; }

        public PixelImage(Vector2Int size)
        {
            Size = size;
            Pixels = new Pixel[size.x, size.y];
        }
    }
}