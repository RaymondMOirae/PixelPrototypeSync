﻿using System;
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
        
        public Vector2Int RenderGridSize { get; private set; }
        
        public Mesh Mesh { get; private set; }

        private SpriteRenderer _spriteRenderer;

        private void Awake()
        {
            Mesh = new Mesh();
            _spriteRenderer = GetComponent<SpriteRenderer>();
            
            if (_imageAsset)
            {
                ReGenerateTexture();
            }
        }

        private void Update()
        {
        }

        public void SetPixelImage(PixelImage image)
        {
            var sprite = Sprite.Create(image.Texture,
                new Rect(0, 0, image.Texture.width, image.Texture.height), Vector2.one / 2);
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
                    new Rect(0, 0, _imageAsset.Image.Texture.width, _imageAsset.Image.Texture.height), Vector2.one / 2);
                _spriteRenderer.sprite = sprite;
            }
        }
    }
}