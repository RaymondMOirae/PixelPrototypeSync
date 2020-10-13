using System;
using System.Collections.Generic;
using Prototype.Element;
using Prototype.UI;
using Script.GameSystem;
using UnityEngine;

namespace Prototype.Script.Test
{
    public class TestPixelEditor : MonoBehaviour
    {
        public int Count = 32;
        
        private List<Pixel> pixels = new List<Pixel>();
        private void Start()
        {
            foreach (var pixelElement in ResourceManager.Instance.PixelElements)
            {
                for (var i = 0; i < Count; i++)
                {
                    pixels.Add(new Pixel(pixelElement));
                }
            }
            
            PixelEditor.Instance.UpdateUI(pixels, new Vector2Int(8, 8));
        }
    }
}