using System;
using System.Collections.Generic;
using System.Linq;
using Prototype.Element;
using Prototype.Inventory;
using Prototype.Rendering;
using Prototype.Settings;
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
        public Inventory.Inventory Inventory;
        private PixelImageRenderer renderer;
        private async void Start()
        {
            Inventory = new Inventory.Inventory();
            foreach (var pixelType in PixelAssets.Current.PixelTypes.Where(p=>p.Attribute == PixelAttribute.Default))
            {
                for (var i = 0; i < Count; i++)
                {
                    Inventory.SaveItem(new Pixel(pixelType));
                }
            }
            
            var image = new PixelImage(8, 8);
            image.Pixels[7, 0] = PixelAssetManager.CreatePixel(PixelAssetManager.FindPixelType("WoodHandel"));
            image.Pixels[6, 1] = PixelAssetManager.CreatePixel(PixelAssetManager.FindPixelType("Wood"));
            image.Pixels[7, 0].Protected = true;
            image.Pixels[6, 1].Protected = true;
            image.UpdateTexture();
            
            Inventory.SaveItem(image);
            
            await PixelEditor.Instance.Edit(Inventory);

            renderer = GameObjectPool.Get<PixelImageRenderer>();
            // image.UpdateTexture();
            renderer.SetPixelImage(image);
            
            // renderer.UpdatePixelImage(image);
        }

        private async void Update()
        {
            if (UnityEngine.Input.GetKeyDown(KeyCode.F1))
            {
                await PixelEditor.Instance.Edit(Inventory);
            }
        }
    }
}