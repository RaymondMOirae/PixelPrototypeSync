using System;
using UnityEngine;

namespace Prototype.Element
{
    [Serializable]
    public class PixelImage
    {
        [SerializeField]
        private Vector2Int _size;

        public Vector2Int Size => _size;
        
        [SerializeField]
        private Pixel[,] _pixels;

        public Pixel[,] Pixels => _pixels;

        public PixelImage(int x, int y) : this(new Vector2Int(x, y))
        {
        }
        public PixelImage(Vector2Int size)
        {
            _size = size;
            _pixels = new Pixel[size.x, size.y];
        }

        public Pixel this[int x, int y]
        {
            get => _pixels[x, y];
            set => _pixels[x, y] = value;
        }

        public static implicit operator bool(PixelImage image)
        {
            return !(image?._pixels is null);
        }
    }
}