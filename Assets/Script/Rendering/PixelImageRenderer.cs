using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Settings;
using Prototype.Utils;
using UnityEngine;

namespace Prototype.Rendering
{
    [RequireComponent(typeof(SpriteRenderer))]
    [ExecuteInEditMode]
    public class PixelImageRenderer : MonoBehaviour, ICustomEditorEX
    {
        public Vector2 RenderSize = Vector2.one;
        
        [SerializeField]
        private PixelImageAsset _imageAsset;
        
        private PixelImage Image { get; set; }
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
                ReGenerateTexture();
            }
        }

        public void SetPixelImage(PixelImage image)
        {
            Image = image;
            Image.UpdateTexture();
            var sprite = Sprite.Create(
                image.Texture,
                new Rect(0, 0, image.Texture.width, image.Texture.height), 
                Vector2.right, 
                image.Texture.width / RenderSize.x);
            transform.localScale = RenderSize.ToVector3(1);
            _spriteRenderer.sprite = sprite;
        }

        public Vector3 ImageToWorldPos(Vector2 imgPos)
        {
            imgPos -= new Vector2(Image.Size.x, 0);
            var localPos = (imgPos / Image.Size).ToVector3(0);
            localPos.x *= _spriteRenderer.flipX ? -1 : 1;
            localPos.y *= _spriteRenderer.flipY ? -1 : 1;
            var worldPos = transform.localToWorldMatrix.MultiplyPoint(localPos);
            return worldPos;
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