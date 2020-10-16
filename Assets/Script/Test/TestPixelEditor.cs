using System;
using System.Collections.Generic;
using System.Linq;
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
            foreach (var pixelElement in PixelAssetManager.Instance.PixelTypes.Where(p=>p.Attribute == PixelAttribute.Default))
            {
                for (var i = 0; i < Count; i++)
                {
                    pixels.Add(new Pixel(pixelElement));
                }
            }
            
            PixelEditor.Instance.SetEditable(new Vector2Int(7, 0), false);
            PixelEditor.Instance.SetEditable(new Vector2Int(6, 1), false);
            
            var image = new PixelImage(8, 8);
            image.Pixels[7, 0] = PixelAssetManager.CreatePixel(PixelAssetManager.FindPixelType("WoodHandel"));
            image.Pixels[6, 1] = PixelAssetManager.CreatePixel(PixelAssetManager.FindPixelType("Wood"));
            
            await PixelEditor.Instance.Edit(pixels, image);

            var renderer = GameObjectPool.Get<PixelImageRenderer>();
            renderer.UpdatePixelImage(image);
        }
    }
}