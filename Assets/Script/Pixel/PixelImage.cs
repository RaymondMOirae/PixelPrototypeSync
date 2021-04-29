using System;
using Prototype.Inventory;
using Prototype.Rendering;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Element
{
    [Serializable]
    public class PixelImage : Item
    {
        [SerializeField]
        private Vector2Int _size;

        public Vector2Int Size => _size;
        
        [SerializeField]
        private Pixel[,] _pixels;

        public Pixel[,] Pixels => _pixels;

        private PixelImageType _itemType;
        public override ItemType ItemType => _itemType;

        public Texture2D Texture => _imageRenderHelper.Texture;
        public RenderTexture RenderTexture => _imageRenderHelper.RenderTexture;

        private readonly PixelImageRenderHelper _imageRenderHelper;

        public PixelImage(int x, int y) : this(new Vector2Int(x, y))
        {
        }
        
        public PixelImage(Vector2Int size)
        {
            _size = size;
            _pixels = new Pixel[size.x, size.y];
            _imageRenderHelper = new PixelImageRenderHelper(size);
            _itemType = ScriptableObjectPool.Get<PixelImageType>();
            _itemType.Init(this);
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

        public void UpdateTexture()
        {
            _imageRenderHelper.UpdateTexture(this);
        }

        // Only use for test
        public PixelImage Clone(bool newPixelInstance = true)
        {
            var image = new PixelImage(this.Size)
            {
                _pixels = _pixels.Clone() as Pixel[,]
            };
            if (newPixelInstance)
            {
                
                for (var y = 0; y < Size.y; y++)
                for (var x = 0; x < Size.x; x++)
                {
                    image._pixels[x, y] = Pixels[x, y] ? new Pixel(Pixels[x, y].Type) : null;
                }
            }
            
            image.UpdateTexture();
            return image;
        }
        

    }
}