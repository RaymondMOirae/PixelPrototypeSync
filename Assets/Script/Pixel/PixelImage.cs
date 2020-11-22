using System;
using Prototype.Inventory;
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
            _imageRenderHelper = new PixelImageRenderHelper(new Vector2Int(x, y));
            _itemType = ScriptableObjectPool.Get<PixelImageType>();
            _itemType.Init(this);
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

        public void UpdateTexture()
        {
            _imageRenderHelper.UpdateTexture(this);
        }

    }
}