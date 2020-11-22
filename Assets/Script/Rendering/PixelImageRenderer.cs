using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Rendering
{
    // [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    [RequireComponent(typeof(SpriteRenderer))]
    public class PixelImageRenderer : MonoBehaviour
    {
        [SerializeField]
        private PixelImageAsset _imageAsset;
        public PixelImage Image { get; private set; }
        
        public Vector2Int RenderGridSize { get; private set; }
        
        public Mesh Mesh { get; private set; }

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            Mesh = new Mesh();

            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Update()
        {
            if (_imageAsset && _spriteRenderer.sprite is null)
            {
                var sprite = Sprite.Create(_imageAsset.Image.Texture,
                    new Rect(0, 0, _imageAsset.Image.Texture.width, _imageAsset.Image.Texture.height), Vector2.one / 2);
                _spriteRenderer.sprite = sprite;
            }
        }
    }
}