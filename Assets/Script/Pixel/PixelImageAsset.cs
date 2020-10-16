using System;
using System.Collections.Generic;
using UnityEngine;

namespace Prototype.Element
{
    [CreateAssetMenu(menuName = "Prototype/PixelImage", fileName = "PixelImage")]
    public class PixelImageAsset : ScriptableObject
    {
        [Serializable]
        class PixelListWrapper
        {
            public Pixel[] Pixels;
            public Pixel this[int index]
            {
                get => Pixels[index];
                set => Pixels[index] = value;
            }

            public PixelListWrapper(int size)
            {
                Pixels = new Pixel[size];
            }
        }
        
        [SerializeField] private int _width = 8;
        [SerializeField] private int _height = 8;

        [SerializeField] private PixelListWrapper[] _pixels = {
            new PixelListWrapper(8),  new PixelListWrapper(8),
            new PixelListWrapper(8),  new PixelListWrapper(8),
            new PixelListWrapper(8),  new PixelListWrapper(8),
            new PixelListWrapper(8),  new PixelListWrapper(8),
        };
        
        public Vector2Int Size => new Vector2Int(_width, _height);
        public int Width => Size.x;
        public int Height => Size.y;

        [SerializeField] private Pixel pixel;

        private PixelImage _image;
        public PixelImage Image
        {
            get => _image is null || _image.Pixels is null
                ? ReGenerateImage()
                : _image;
            set
            {
                _image = value;
                _width = _image.Size.x;
                _height = _image.Size.y;
                _pixels = new PixelListWrapper[_height];
                for (var y = 0; y < _height; y++)
                {
                    _pixels[y] = new PixelListWrapper(_width);
                    for (var x = 0; x < _width; x++)
                    {
                        _pixels[y][x] = _image.Pixels[x, y];
                        if (_image.Pixels[x, y] != null)
                            pixel = _image.Pixels[x, y];
                    }
                }
            }
        }

        public PixelImage ReGenerateImage()
        {
            _image = new PixelImage(_width, _height);
            for (var y = 0; y < _height; y++)
            for (var x = 0; x < _width; x++)
            {
                _image.Pixels[x, y] = _pixels[y][x];
            }

            return _image;
        }

        public void Resize(int width, int height)
        {
            Image = new PixelImage(width, height);
        }

        public void Resize(Vector2Int size)
            => Resize(size.x, size.y);

    }
}