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
    [ExecuteInEditMode]
    public class PixelImageRenderer : MonoBehaviour, ICustomEditorEX
    {
        [SerializeField]
        private PixelImageAsset _imageAsset;
        public PixelImage Image { get; private set; }
        public Sprite Sprite{get{ return _spriteRenderer.sprite; } }

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        private void Start()
        {
            if (_imageAsset)
            {
                Image = _imageAsset.Image;
                Image.UpdateTexture();
                ReGenerateTexture();
            }
        }

        public void SetPixelImage(PixelImage image)
        {
            Image = image;
            Image.UpdateTexture();
            var sprite = Sprite.Create(image.Texture,
                new Rect(0, 0, image.Texture.width, image.Texture.height), Vector2.right);
            _spriteRenderer.sprite = sprite;
        }

        [EditorButton]
        void UpdateTexture()
        {
            if(_imageAsset)
                _imageAsset.Image.UpdateTexture();
        }

        [EditorButton()]
        void ReGenerateTexture()
        {
            if (_imageAsset)
            {
                _imageAsset.ReGenerateImage();
                var sprite = Sprite.Create(_imageAsset.Image.Texture,
                    new Rect(0, 0, _imageAsset.Image.Texture.width, _imageAsset.Image.Texture.height), Vector2.right);
                _spriteRenderer.sprite = sprite;
            }
        }
    }
}