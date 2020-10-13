using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.Rendering;
using Prototype.UI;
using Prototype.Utils;
using Script.GameSystem;
using UnityEngine;

namespace Prototype.Script.Test
{
    public class TestPixelEditor : MonoBehaviour
    {
        public int Count = 32;
        
        private List<Pixel> pixels = new List<Pixel>();
        private async void Start()
        {
            foreach (var pixelElement in ResourceManager.Instance.PixelElements)
            {
                for (var i = 0; i < Count; i++)
                {
                    pixels.Add(new Pixel(pixelElement));
                }
            }
            
            var image = await PixelEditor.Instance.Edit(pixels, new Vector2Int(8, 8));

            var renderer = GameObjectPool.Get<PixelImageRenderer>();
            renderer.UpdatePixelImage(image);
        }
    }
}